using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dal;
using Dal.Common;
using Dal.Wrappers;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;


namespace UnitTestProject
{
    [TestFixture]
    public class PlayersProviderTest
    {
        [Test]
        public async Task SuccessfulLoadPlayers()
        {
            // Arange.
            var dbProvider = new Mock<IMongoDbProvider>();
            var defBuilder = new Mock<IMongoDbDefinitionBuilder>();
            var collection = new Mock<IMongoBsonCollection>();
            var db = new Mock<IMongoDatabase>();
            var encryptor = new Mock<IEncryptor>();

            dbProvider.Setup(d => d.GetDatabase(It.IsAny<string>())).Returns(db.Object);
            dbProvider.Setup(d => d.GetBsonDocumentCollection(It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(collection.Object);
            
            var bsonList = new List<BsonDocument>();
            var document = new BsonDocument
            {
                {"Id", 0},
                {"PersonId", "SuperCoolPlayer"},
                {"Nickname", "vasya228"},
                {"Country", "Peedorusha"},
                {"Money", 228},
                {"ScoreMax", 1488},
                {"LastSessionDateTime", new DateTime(2015, 09, 23)},
                {"FirstSessionDateTime", new DateTime(2015, 04, 22)},
                {"SelectedSkin", 0},
                {"UnlockedSkins", "[0,1,2,3]"},
                {"SelectedAbility", 0},
                {"UnlockedAbilities", "[0,1,2,3]"}
            };
            bsonList.Add(document);

            collection.Setup(c => c.GetDocumentsListAsync()).ReturnsAsync(bsonList);

            var pp = new PlayersProvider(dbProvider.Object, defBuilder.Object, encryptor.Object);

            // Act.
            var playersList = await pp.LoadPlayers();

            // Assert.
            Assert.AreEqual(1, playersList.Count);
            Assert.AreEqual(document["Id"].AsInt32, playersList.First().Id);
            Assert.AreEqual(document["PersonId"].AsString, playersList.First().PersonId);
            Assert.AreEqual(document["Nickname"].AsString, playersList.First().Nickname);
            Assert.AreEqual(document["Country"].AsString, playersList.First().Country);
            Assert.AreEqual(document["Money"].AsInt32, playersList.First().Money);
            Assert.AreEqual(document["ScoreMax"].AsInt32, playersList.First().ScoreMax);
            Assert.AreEqual(document["LastSessionDateTime"].ToLocalTime(), playersList.First().LastSessionDateTime);
            Assert.AreEqual(document["FirstSessionDateTime"].ToLocalTime(), playersList.First().FirstSessionDateTime);
            Assert.AreEqual((short)document["SelectedSkin"].AsInt32, playersList.First().SelectedSkin);
            Assert.AreEqual((short)document["SelectedAbility"].AsInt32, playersList.First().SelectedAbility);
            Assert.AreEqual("0, 1, 2, 3", playersList.First().UnlockedAbilitiesString);
            Assert.AreEqual("0, 1, 2, 3", playersList.First().UnlockedSkinsString);
        }

        [Test]
        public async Task SuccessfulEditPlayer()
        {
            // Arange.
            var dbProvider = new Mock<IMongoDbProvider>();
            var defBuilder = new Mock<IMongoDbDefinitionBuilder>();
            var collection = new Mock<IMongoBsonCollection>();
            var db = new Mock<IMongoDatabase>();
            var encryptor = new Mock<IEncryptor>();

            dbProvider.Setup(d => d.GetDatabase(It.IsAny<string>())).Returns(db.Object);
            dbProvider.Setup(d => d.GetBsonDocumentCollection(It.IsAny<string>(), It.IsAny<string>()))
                      .Returns(collection.Object);

            var resultValue = "";

            defBuilder.Setup(d => d.GetFilterForPlayerEdit(It.IsAny<int>()))
                      .Returns(default(FilterDefinition<BsonDocument>));
            defBuilder.Setup(d => d.GetUpdateForPlayerEdit(It.IsAny<string>(), It.IsAny<object>()))
                      .Callback((string p, object n) => resultValue = n as string)
                      .Returns(default(UpdateDefinition<BsonDocument>));
            
            var updateResult = new Mock<UpdateResult>();
            updateResult.Setup(u => u.ModifiedCount).Returns(1);

            collection.Setup(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<UpdateDefinition<BsonDocument>>()))
                .ReturnsAsync(updateResult.Object);
            
            var pp = new PlayersProvider(dbProvider.Object, defBuilder.Object, encryptor.Object);

            const int playerId = 0;
            const string parameterToUpdate = "UnlockedSkins";
            const string newValue = "0, 1, 2, 3, 4";

            // Act.
            var success = await pp.EditPlayer(playerId, parameterToUpdate, newValue);

            // Assert.
            Assert.True(success);
            Assert.AreEqual("[0,1,2,3,4]", resultValue);
        }
    }
}