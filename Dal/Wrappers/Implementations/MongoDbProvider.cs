using System;
using MongoDB.Bson;
using MongoDB.Driver;


namespace Dal.Wrappers.Implementations
{
    public class MongoDbProvider : IMongoDbProvider
    {
        private readonly IErrorHandler errorHandler;
        public IMongoClient Client { get; }

        public MongoDbProvider(IErrorHandler errorHandler)
        {
            if (errorHandler == null)
                throw new ArgumentNullException(nameof(errorHandler));

            this.errorHandler = errorHandler;

            Client = new MongoClient("mongodb://localhost:27017");
        }

        public IMongoDatabase GetDatabase(string databaseName)
        {
            return Client.GetDatabase(databaseName);
        }

        public IMongoBsonCollection GetBsonDocumentCollection(string databaseName, string collectionName)
        {
            return new MongoBsonCollection(Client.GetDatabase(databaseName).GetCollection<BsonDocument>(collectionName), errorHandler);
        }
    }
}