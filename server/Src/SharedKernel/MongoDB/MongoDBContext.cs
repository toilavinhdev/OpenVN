using MongoDB.Driver;
using SharedKernel.Libraries;

namespace SharedKernel.MongoDB
{
    public class MongoDBContext
    {
        #region Declares + Constructor
        private readonly IMongoDatabase db;
        private readonly IMongoClient mongoClient;

        public IMongoClient OriginClient => mongoClient;

        public MongoDBContext(string connectionString, string databaseName)
        {
            mongoClient = new MongoClient(connectionString);
            db = mongoClient.GetDatabase(databaseName);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get collection
        /// </summary>
        public IMongoCollection<TDocument> GetCollection<TDocument>()
        {
            string collectionName = typeof(TDocument).Name.ToSnakeCaseLower();
            return db.GetCollection<TDocument>(collectionName);
        }
        #endregion
    }
}
