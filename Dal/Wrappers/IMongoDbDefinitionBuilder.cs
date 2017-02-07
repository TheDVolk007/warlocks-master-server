using System;
using Domain;
using MongoDB.Bson;
using MongoDB.Driver;


namespace Dal.Wrappers
{
    public interface IMongoDbDefinitionBuilder
    {
        FilterDefinition<BsonDocument> GetComplexFilterForServerUpdate(ServerInfo serverInfo);
        FilterDefinition<BsonDocument> GetFilterForUnactiveServer(TimeSpan maxServerLifetime);
        UpdateDefinition<BsonDocument> GetComplexUpdateForServerUpdate(ServerInfo serverInfo);
        FilterDefinition<BsonDocument> GetFilterForPlayerEdit(int playerId);
        UpdateDefinition<BsonDocument> GetUpdateForPlayerEdit(string parameterToUpdate, object newValue);
    }
}