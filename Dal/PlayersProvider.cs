using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Dal.Common;
using Dal.Wrappers;
using Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;


namespace Dal
{
    public class PlayersProvider : IPlayersProvider
    {
        private readonly IMongoDbProvider dbProvider;
        private readonly IMongoDbDefinitionBuilder mongoDbDefinitionBuilder;
        private readonly IEncryptor encryptor;
        private const string DatabaseName = "Players";
        private const string CollectionName = "Players";

        private readonly List<PlayerEntity> playersList = new List<PlayerEntity>();
        private IMongoDatabase playersDatabase;
        private IMongoBsonCollection playersCollection;

        public PlayersProvider(IMongoDbProvider dbProvider,
            IMongoDbDefinitionBuilder mongoDbDefinitionBuilder,
            IEncryptor encryptor)
        {
            this.dbProvider = dbProvider;
            this.mongoDbDefinitionBuilder = mongoDbDefinitionBuilder;
            this.encryptor = encryptor;
            playersDatabase = dbProvider.GetDatabase(DatabaseName);
            playersCollection = dbProvider.GetBsonDocumentCollection(DatabaseName, CollectionName);
        }

        public async Task<List<PlayerEntity>> LoadPlayers()
        {
            var documents = await playersCollection.GetDocumentsListAsync();
            playersList.Clear();

            foreach (var document in documents)
            {
                // Unlocked Skins
                var unlockedSkins = JsonConvert.DeserializeObject<List<short>>(
                    document["UnlockedSkins"].AsString);

                // Unlocked Abilities
                var unlockedAbilities = JsonConvert.DeserializeObject<List<short>>(
                    document["UnlockedAbilities"].AsString);

                var player = new PlayerEntity(unlockedSkins, unlockedAbilities)
                {
                    Id = document["Id"].AsInt32,
                    PersonId = document["PersonId"].AsString,
                    Nickname = document["Nickname"].AsString,
                    Country = document["Country"].AsString,
                    Money = document["Money"].AsInt32,
                    ScoreMax = document["ScoreMax"].AsInt32,
                    LastSessionDateTime = document["LastSessionDateTime"].ToLocalTime(),
                    FirstSessionDateTime = document["FirstSessionDateTime"].ToLocalTime(),
                    SelectedSkin = (short)document["SelectedSkin"].AsInt32,
                    SelectedAbility = (short)document["SelectedAbility"].AsInt32
                };
                playersList.Add(player);
            }

            return playersList;
        }

        public async Task<bool> EditPlayer(int playerId, string parameterToUpdate, object newValue)
        {
            var filter = mongoDbDefinitionBuilder.GetFilterForPlayerEdit(playerId);

            if(parameterToUpdate == "UnlockedSkins" || parameterToUpdate == "UnlockedAbilities")
            {
                var items = newValue.ToString().Split(
                    new[] {',', ' ', '.'},
                    StringSplitOptions.RemoveEmptyEntries);

                var unlockedItems = new List<short>();
                foreach(var item in items)
                {
                    unlockedItems.Add(short.Parse(item));
                }
                unlockedItems.Sort();

                newValue = JsonConvert.SerializeObject(unlockedItems);
            }

            var update = mongoDbDefinitionBuilder.GetUpdateForPlayerEdit(parameterToUpdate, newValue);
            
            var updateResult = await playersCollection.UpdateOneAsync(filter, update);

            return updateResult.ModifiedCount > 0;
        }

        public async void TestFillDB()
        {
            playersDatabase = dbProvider.GetDatabase(DatabaseName);
            playersCollection = dbProvider.GetBsonDocumentCollection(DatabaseName, CollectionName);

            await playersDatabase.DropCollectionAsync(CollectionName); // очистка коллекции БД

            var random = new Random(0);

            for (var i = 0; i < 50; i++)
            {
                // Unlocked Skins
                var unlockedSkins = new List<short>();
                for (var j = 0; j < (i + 17) / 20; j++)
                {
                    unlockedSkins.Add((short)(random.Next(0, 15)));
                }
                unlockedSkins.Sort();

                var unlockedSkinsJson = JsonConvert.SerializeObject(unlockedSkins);

                // Unlocked Abilities
                var unlockedAbilities = new List<short>();
                for (var j = 0; j < (i + 8) / 10; j++)
                {
                    unlockedAbilities.Add((short)(random.Next(0, 35)));
                }
                unlockedAbilities.Sort();

                var unlockedAbilitiesJson = JsonConvert.SerializeObject(unlockedAbilities);

                var doc = new BsonDocument
                {
                    {"Id", i},
                    {"PersonId", encryptor.SHA256($"email{i}@gmail.com")},
                    {"Nickname", $"Player random-n{i - 102*((-3) ^ (i/5))}"},
                    {"Country", CultureInfo.GetCultures(CultureTypes.SpecificCultures)[i*7].Name},
                    {"Money", 2250 - 20*i - (-2) ^ (i/5)},
                    {"ScoreMax", 1150 + (-3) ^ (i/5)},
                    {"LastSessionDateTime", DateTime.Now.AddMinutes(-1.25f*i)},
                    {"FirstSessionDateTime", DateTime.Now.AddDays(-50f/(i + 1))},
                    {"SelectedSkin", i/4},
                    {"UnlockedSkins", unlockedSkinsJson},
                    {"SelectedAbility", 25/(i + 1)},
                    {"UnlockedAbilities", unlockedAbilitiesJson}
                };

                playersCollection.InsertOneAsync(doc);
            }

            //MessageBox.Show($"[{CollectionName}]: Test filling completed.\nComment this function and restart application",
            //    "Done", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //MediaTypeNames.Application.Exit();
        }
    }
}
