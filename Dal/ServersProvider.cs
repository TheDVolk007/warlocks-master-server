using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dal.Common;
using Dal.Wrappers;
using Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;


namespace Dal
{
    public class ServersProvider : IServersProvider
    {
        private readonly IMongoDbProvider dbProvider;
        private readonly IMongoDbDefinitionBuilder mongoDbDefinitionBuilder;
        private readonly IEncryptor encryptor;
        private const string DatabaseName = "Servers";
        private const string CollectionName = "Servers";

        private readonly List<ServerEntity> serversList = new List<ServerEntity>();
        private IMongoDatabase serversDatabase;
        private IMongoBsonCollection serversCollection;

        private readonly static TimeSpan MaxServerLifetime = new TimeSpan(0, 0, 1, 30); // сервера, не обновлявшиеся более 1:30, удаляются

        public event UpdateHandler OnUpdateServers;

        public ServersProvider(IMongoDbProvider dbProvider,
            IMongoDbDefinitionBuilder mongoDbDefinitionBuilder,
            IEncryptor encryptor)
        {
            this.dbProvider = dbProvider;
            this.mongoDbDefinitionBuilder = mongoDbDefinitionBuilder;
            this.encryptor = encryptor;
            serversDatabase = dbProvider.GetDatabase(DatabaseName);
            serversCollection = dbProvider.GetBsonDocumentCollection(DatabaseName, CollectionName);
        }

        public async Task<List<ServerEntity>> LoadServers()
        {
            var documents = await serversCollection.GetDocumentsListAsync();
            serversList.Clear();

            foreach (var document in documents)
            {
                var server = new ServerEntity
                {
                    Id = document["Id"].AsInt32,
                    Name = document["Name"].AsString,
                    Ip = document["IP"].AsString,
                    Port = document["Port"].AsInt32,
                    Region = (Region)document["Region"].AsInt32,
                    PlayersCount = document["PlayersCount"].AsInt32,
                    PlayersMax = document["PlayersMax"].AsInt32,
                    MinutesToNextLevelChange = document["MinutesToNextLevelChange"].AsInt32,
                    LastUpdateDateTime = document["LastUpdateDateTime"].ToLocalTime()
                };
                serversList.Add(server);
            }

            //serversList = serversList.OrderBy(s => s.Id).ToList();
            return serversList;
        }

        public async Task<long> DeleteUnactiveServers()
        {
            // удаляем давно не обновлявшиеся серваки из списка
            var filter = mongoDbDefinitionBuilder.GetFilterForUnactiveServer(MaxServerLifetime);
            // также можно найти все документы (deletedServers), соответствующие условию, чтобы были более подробные логи
            var result = await serversCollection.DeleteManyAsync(filter);

            if (OnUpdateServers != null)
            {
                OnUpdateServers(result.DeletedCount > 0);
            }
            
            return result.DeletedCount;
        }

        //public static async void EditServer(int serverid, string parameterToUpdate, object newValue)
        //{
        //    var filter = Builders<BsonDocument>.Filter.Eq("Id", serverid);
        //    var update = Builders<BsonDocument>.Update.Set(parameterToUpdate, newValue);

        //    UpdateResult updateResult = await serversCollection.UpdateOneAsync(filter, update);
        //    if (updateResult.ModifiedCount > 0)
        //        MessageBox.Show($"[{CollectionName}]:\nModified count = [{updateResult.ModifiedCount}]\n" +
        //                        $"Server: [{serverid}]\nValue of [{parameterToUpdate}] changed to [{newValue}]",
        //            "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //}


        /// <summary>
        /// Возвращает true, если был обновлен существующий сервер, и false, если была создана новая запись в базу
        /// </summary>
        /// <param name="serverInfo"></param>
        public async Task<bool> UpdateServer(ServerInfo serverInfo)
        {
            var finalFilter = mongoDbDefinitionBuilder.GetComplexFilterForServerUpdate(serverInfo);
            var finalUpdate = mongoDbDefinitionBuilder.GetComplexUpdateForServerUpdate(serverInfo);

            var updateResult = await serversCollection.UpdateOneAsync(finalFilter, finalUpdate);
            // если ни одного сервера обновлено не было, то добавляем новый
            if (updateResult.ModifiedCount == 0)
            {
                var doc = new BsonDocument
                {
                    {"Id", GetAvailableServerId()},
                    {"Name", serverInfo.Name},
                    {"IP", serverInfo.Ip},
                    {"Port", serverInfo.Port},
                    {"Region", serverInfo.Region},
                    {"PlayersCount", serverInfo.PlayersCount},
                    {"PlayersMax", serverInfo.PlayersMax},
                    {"MinutesToNextLevelChange", serverInfo.MinutesToNextLevelChange},
                    {"LastUpdateDateTime", DateTime.Now}
                };

                serversCollection.InsertOneAsync(doc);

                //MongoDBForm.SetStatusStripMessage($"Added new server: {serverInfo.Name} - {serverInfo.Ip}:{serverInfo.Port} ({serverInfo.Region})");
            }

            if (OnUpdateServers != null)
            {
                OnUpdateServers(updateResult.ModifiedCount == 0); // если добавили новый (нет изменённых), то полностью обновляем таблицу
            }
            return updateResult.ModifiedCount != 0;
        }

        private int GetAvailableServerId()
        {
            var ids = new List<int>();
            foreach (var server in serversList)
            {
                ids.Add(server.Id);
            }
            ids.Sort();
            for (var i = 0; i < serversList.Count - 1; i++)
            {
                if (ids[i + 1] - ids[i] > 1)
                    return ids[i] + 1;
            }

            return (ids.Count > 0) ? (ids[ids.Count - 1] + 1) : 0;
        }

        public async void TestFillDB()
        {
            serversDatabase = dbProvider.GetDatabase(DatabaseName);
            serversCollection = dbProvider.GetBsonDocumentCollection(DatabaseName, CollectionName);

            await serversDatabase.DropCollectionAsync(CollectionName); // очистка коллекции БД

            for (var i = 0; i < 15; i++)
            {
                var doc = new BsonDocument
                {
                    {"Id", i > 3 && i <= 5 ? i + 30 : i},
                    {"Name", $"Server number {-2*((-2) ^ i)}"},
                    {"IP", "127.0.0.1"},//$"127.0.{2*i}.{50 - i}"},
                    {"Port", 7777},//7777 - i},
                    {"Region", i},
                    {"PlayersCount", 95 - i},
                    {"PlayersMax", 100},
                    {"MinutesToNextLevelChange", 120 - i},
                    {"LastUpdateDateTime", DateTime.Now.AddMinutes(- 2 - (-2) ^ i).AddDays(1)}
                };

                await serversCollection.Collection.InsertOneAsync(doc);
            }

            //MessageBox.Show($"[{CollectionName}]: Test filling completed.\nComment this function and restart application",
            //    "Done", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //MediaTypeNames.Application.Exit();
        }

        public string SelectServerForPlayerJson(PlayerInfo playerInfo)
        {
            var region = RegionByCountry.GetClosestRegion(playerInfo.Country);

            // используя region выбрать из ближайших серверов самый подходящий
            var serverInfo = new ServerInfo
            {
                Name = serversList[0].Name, // [0] - DEBUG ONLY
                Ip = serversList[0].Ip,
                Port = serversList[0].Port,
                Region = serversList[0].Region,
                PlayersCount = serversList[0].PlayersCount,
                PlayersMax = serversList[0].PlayersMax,
                MinutesToNextLevelChange = serversList[0].MinutesToNextLevelChange
            };

            return JsonConvert.SerializeObject(serverInfo);
        }
    }
}
