using System;
using Domain;
using MongoDB.Bson;
using MongoDB.Driver;


namespace Dal.Wrappers.Implementations
{
    public class MongoDbDefinitionBuilder : IMongoDbDefinitionBuilder
    {
        public FilterDefinition<BsonDocument> GetComplexFilterForServerUpdate(ServerInfo serverInfo)
        {
            var filter0 = Builders<BsonDocument>.Filter.Eq("Name", serverInfo.Name);
            var filter1 = Builders<BsonDocument>.Filter.Eq("IP", serverInfo.Ip);
            var filter2 = Builders<BsonDocument>.Filter.Eq("Port", serverInfo.Port);
            var finalFilter = Builders<BsonDocument>.Filter.And(filter0, filter1, filter2);
            return finalFilter;
        }

        public UpdateDefinition<BsonDocument> GetComplexUpdateForServerUpdate(ServerInfo serverInfo)
        {
            var update0 = Builders<BsonDocument>.Update.Set("PlayersCount", serverInfo.PlayersCount);
            var update1 = Builders<BsonDocument>.Update.Set("PlayersMax", serverInfo.PlayersMax);
            var update2 = Builders<BsonDocument>.Update.Set("MinutesToNextLevelChange", serverInfo.MinutesToNextLevelChange);
            var update3 = Builders<BsonDocument>.Update.Set("LastUpdateDateTime", DateTime.Now);
            var finalUpdate = Builders<BsonDocument>.Update.Combine(update0, update1, update2, update3);
            return finalUpdate;
        }

        public FilterDefinition<BsonDocument> GetFilterForUnactiveServer(TimeSpan maxServerLifetime)
        {
            var t = DateTime.Now.ToLocalTime().Add(-maxServerLifetime);
            var filter = Builders<BsonDocument>.Filter.Lt("LastUpdateDateTime", t);
            return filter;
        }

        public FilterDefinition<BsonDocument> GetFilterForPlayerEdit(int playerId)
        {
            return Builders<BsonDocument>.Filter.Eq("Id", playerId);
        }

        public UpdateDefinition<BsonDocument> GetUpdateForPlayerEdit(string parameterToUpdate, object newValue)
        {
            return Builders<BsonDocument>.Update.Set(parameterToUpdate, newValue);
        }
    }
}