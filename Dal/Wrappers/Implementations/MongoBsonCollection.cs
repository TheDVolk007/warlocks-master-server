using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;


namespace Dal.Wrappers.Implementations
{
    public class MongoBsonCollection : IMongoBsonCollection
    {
        private readonly IErrorHandler errorHandler;
        public IMongoCollection<BsonDocument> Collection { get; }


        public MongoBsonCollection(IMongoCollection<BsonDocument> collection, IErrorHandler errorHandler)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (errorHandler == null)
                throw new ArgumentNullException(nameof(errorHandler));

            this.errorHandler = errorHandler;
            Collection = collection;
        }

        public async Task<List<BsonDocument>> GetDocumentsListAsync()
        {
            try
            {
                return await Collection.Find(new BsonDocument()).ToListAsync();
            }
            catch (Exception e)
            {
                errorHandler.HandleError(e);
                return new List<BsonDocument>();
            }
        }

        public async Task<DeleteResult> DeleteManyAsync(FilterDefinition<BsonDocument> filter)
        {
            try
            {
                return await Collection.DeleteManyAsync(filter);
            }
            catch (Exception e)
            {
                errorHandler.HandleError(e);
                return new DeleteResult.Acknowledged(0);
            }
        }

        public async Task<UpdateResult> UpdateOneAsync(FilterDefinition<BsonDocument> filter,
                                                       UpdateDefinition<BsonDocument> update)
        {
            return await Collection.UpdateOneAsync(filter, update);
        }

        public async void InsertOneAsync(BsonDocument document)
        {
            await Collection.InsertOneAsync(document);
        }
    }
}