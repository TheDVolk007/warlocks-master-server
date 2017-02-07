using MongoDB.Driver;


namespace Dal.Wrappers
{
    public interface IMongoDbProvider
    {
        IMongoClient Client { get; }
        IMongoBsonCollection GetBsonDocumentCollection(string databaseName, string collectionName);
        IMongoDatabase GetDatabase(string databaseName);
    }
}