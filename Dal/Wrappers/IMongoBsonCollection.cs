using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;


namespace Dal.Wrappers
{
    public interface IMongoBsonCollection
    {
        IMongoCollection<BsonDocument> Collection { get; }

        Task<List<BsonDocument>> GetDocumentsListAsync();

        Task<DeleteResult> DeleteManyAsync(FilterDefinition<BsonDocument> filter);

        Task<UpdateResult> UpdateOneAsync(FilterDefinition<BsonDocument> filter,
                                          UpdateDefinition<BsonDocument> update);

        void InsertOneAsync(BsonDocument document);
    }

    //public interface Ilame
    //{
    //    Task<string> GetString(string s);
    //}
}