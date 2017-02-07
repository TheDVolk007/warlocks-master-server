using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dal;
using Dal.Common;
using Dal.Wrappers;
using Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;


namespace UnitTestProject
{
    [TestFixture]
    public class ServersProviderTest
    {
        [Test]
        public async Task SuccessfulLoadServer()
        {
            // Arange.  
            var dbProvider = new Mock<IMongoDbProvider>();
            var db = new Mock<IMongoDatabase>();
            var collection = new Mock<IMongoBsonCollection>();
            var defBuilder = new Mock<IMongoDbDefinitionBuilder>();
            var encryptor = new Mock<IEncryptor>();

            dbProvider.Setup(d => d.GetDatabase(It.IsAny<string>())).Returns(db.Object);
            dbProvider.Setup(d => d.GetBsonDocumentCollection(It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(collection.Object);

            var bsonList = new List<BsonDocument>();
            var document = new BsonDocument
            {
                {"Id", 0},
                {"Name", "SuperCoolServer"},
                {"IP", "10.1.5.111"},
                {"Port", 9001},
                {"Region", 1}, //EastUS
                {"PlayersCount", 3},
                {"PlayersMax", 100},
                {"MinutesToNextLevelChange", 10},
                {"LastUpdateDateTime", new DateTime(2015, 9, 21, 12, 0, 0)}
            };
            bsonList.Add(document);

            collection.Setup(c => c.GetDocumentsListAsync()).ReturnsAsync(bsonList);

            var sp = new ServersProvider(dbProvider.Object, defBuilder.Object, encryptor.Object);

            // Act.
            var serversList = await sp.LoadServers();

            // Assert.
            Assert.AreEqual(1, serversList.Count);
            Assert.AreEqual(document["Id"].AsInt32, serversList.First().Id);
            Assert.AreEqual(document["Name"].AsString, serversList.First().Name);
            Assert.AreEqual(document["IP"].AsString, serversList.First().Ip);
            Assert.AreEqual(document["Port"].AsInt32, serversList.First().Port);
            Assert.AreEqual((Region)document["Region"].AsInt32, serversList.First().Region);
            Assert.AreEqual(document["PlayersCount"].AsInt32, serversList.First().PlayersCount);
            Assert.AreEqual(document["PlayersMax"].AsInt32, serversList.First().PlayersMax);
            Assert.AreEqual(document["MinutesToNextLevelChange"].AsInt32, serversList.First().MinutesToNextLevelChange);
            Assert.AreEqual(document["LastUpdateDateTime"].ToLocalTime(), serversList.First().LastUpdateDateTime);
        }
        
        [Test]
        public async Task SuccessfulDeleteUnactiveServers()
        {
            // Arange.  
            var dbProvider = new Mock<IMongoDbProvider>();
            var db = new Mock<IMongoDatabase>();
            var collection = new Mock<IMongoBsonCollection>();
            var defBuilder = new Mock<IMongoDbDefinitionBuilder>();
            var encryptor = new Mock<IEncryptor>();

            dbProvider.Setup(d => d.GetDatabase(It.IsAny<string>())).Returns(db.Object);
            dbProvider.Setup(d => d.GetBsonDocumentCollection(It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(collection.Object);

            defBuilder.Setup(d => d.GetFilterForUnactiveServer(It.IsAny<TimeSpan>()))
                      .Returns(default(FilterDefinition<BsonDocument>));

            var bsonList = new List<BsonDocument>();
            var document0 = new BsonDocument
            {
                {"Id", 0},
                {"Name", "SuperCoolServer"},
                {"IP", "10.1.5.111"},
                {"Port", 9001},
                {"Region", 1}, //EastUS
                {"PlayersCount", 3},
                {"PlayersMax", 100},
                {"MinutesToNextLevelChange", 10},
                {"LastUpdateDateTime", new DateTime(2015, 9, 21, 12, 0, 0)}
            };
            var document1 = new BsonDocument
            {
                {"Id", 1},
                {"Name", "SuperCoolServerToo"},
                {"IP", "10.1.5.111"},
                {"Port", 9002},
                {"Region", 1}, //EastUS
                {"PlayersCount", 4},
                {"PlayersMax", 100},
                {"MinutesToNextLevelChange", 12},
                {"LastUpdateDateTime", DateTime.Now}
            };
            bsonList.Add(document0);
            bsonList.Add(document1);

            collection.Setup(c => c.GetDocumentsListAsync()).ReturnsAsync(bsonList);
            collection.Setup(c => c.DeleteManyAsync(It.IsAny<FilterDefinition<BsonDocument>>()))
                      .Returns((FilterDefinition<BsonDocument> f) => Task.FromResult(MakeSomeDeletions(f, bsonList)));

            var sp = new ServersProvider(dbProvider.Object, defBuilder.Object, encryptor.Object);
            var eventResult = false;
            sp.OnUpdateServers += delegate (bool clearing) { eventResult = clearing; };

            // Act.
            var deleteCount = await sp.DeleteUnactiveServers();

            // Assert.
            Assert.AreEqual(1, deleteCount);
            Assert.True(eventResult);
        }

        [Test]
        public async Task SuccessfulUpdateServerWithoutBreach()
        {
            await UpdateServer(1);
        }

        [Test]
        public async Task SuccessfulUpdateServerWithBreach()
        {
            await UpdateServer(2);
        }

        private static async Task UpdateServer(int idOfSecondServer)
        {
            // Arange.  
            var dbProvider = new Mock<IMongoDbProvider>();
            var db = new Mock<IMongoDatabase>();
            var collection = new Mock<IMongoBsonCollection>();
            var defBuilder = new Mock<IMongoDbDefinitionBuilder>();
            var encryptor = new Mock<IEncryptor>();

            dbProvider.Setup(d => d.GetDatabase(It.IsAny<string>())).Returns(db.Object);
            dbProvider.Setup(d => d.GetBsonDocumentCollection(It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(collection.Object);

            defBuilder.Setup(d => d.GetComplexFilterForServerUpdate(It.IsAny<ServerInfo>()))
                      .Returns(default(FilterDefinition<BsonDocument>));
            defBuilder.Setup(d => d.GetComplexUpdateForServerUpdate(It.IsAny<ServerInfo>()))
                      .Returns(default(UpdateDefinition<BsonDocument>));

            var bsonList = new List<BsonDocument>();
            var document0 = new BsonDocument
            {
                {"Id", 0},
                {"Name", "SuperCoolServer"},
                {"IP", "10.1.5.111"},
                {"Port", 9001},
                {"Region", 1}, //EastUS
                {"PlayersCount", 3},
                {"PlayersMax", 100},
                {"MinutesToNextLevelChange", 10},
                {"LastUpdateDateTime", new DateTime(2015, 9, 21, 12, 0, 0)}
            };
            var document1 = new BsonDocument
            {
                {"Id", idOfSecondServer},
                {"Name", "SuperCoolServerToo"},
                {"IP", "10.1.5.111"},
                {"Port", 9002},
                {"Region", 1}, //EastUS
                {"PlayersCount", 4},
                {"PlayersMax", 100},
                {"MinutesToNextLevelChange", 12},
                {"LastUpdateDateTime", DateTime.Now}
            };
            bsonList.Add(document0);
            bsonList.Add(document1);

            collection.Setup(c => c.GetDocumentsListAsync()).ReturnsAsync(bsonList);
            var updateResult = new Mock<UpdateResult>();
            updateResult.Setup(u => u.ModifiedCount).Returns(0);

            collection.Setup(
                c => c.UpdateOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<UpdateDefinition<BsonDocument>>()))
                      .ReturnsAsync(updateResult.Object);

            collection.Setup(c => c.InsertOneAsync(It.IsAny<BsonDocument>()))
                      .Callback((BsonDocument bdoc) => bsonList.Add(bdoc));

            var sp = new ServersProvider(dbProvider.Object, defBuilder.Object, encryptor.Object);
            var eventResult = false;
            sp.OnUpdateServers += delegate(bool clearing) { eventResult = clearing; };

            var serverInfo = new ServerInfo
            {
                Name = "SecondServer",
                Ip = "111.5.1.01",
                Port = 9002,
                Region = Region.EastUS,
                PlayersCount = 7,
                PlayersMax = 100,
                MinutesToNextLevelChange = 120
            };

            // Act.
            await sp.LoadServers(); //additional act, should be OK if SuccessfulLoadServer passes
            var newEntry = !(await sp.UpdateServer(serverInfo)); //primary act

            // !!!GetAvailableServerId!!! especially with 1 server\\
            // Assert.
            Assert.True(newEntry);
            Assert.True(eventResult);
            Assert.AreEqual(3, bsonList.Count);
        }

        private static DeleteResult MakeSomeDeletions(FilterDefinition<BsonDocument> f, List<BsonDocument> list)
        {
            var listCountBeforeDeletions = list.Count;
            var deleteResult = new Mock<DeleteResult>();
            var resultList = list.Where(i =>
                i["LastUpdateDateTime"].ToLocalTime()
                > DateTime.Now.ToLocalTime().Add(-new TimeSpan(0, 0, 1, 30))).ToList();
            list = resultList;
            var listCountAfterDeletions = list.Count;

            deleteResult.Setup(d => d.DeletedCount).Returns(listCountBeforeDeletions - listCountAfterDeletions);
            
            return deleteResult.Object;
        }

        private static object GetInstanceField(Type type, object instance, string fieldName)
        {
            const BindingFlags bindFlags = BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static;

            var field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }
    }
}