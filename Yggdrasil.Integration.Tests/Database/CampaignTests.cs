using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Yggdrasil.Server.Configuration;
using Yggdrasil.Server.Storage;
using Yggdrasil.Server.Storage.Mongo;

namespace Yggdrasil.Integration.Tests.Database
{
    [TestFixture]
    public sealed class CampaignTests
    {
        private ICampaignStorage _storage;
        private MongoClient _client;
        private IMongoDatabase _database;

        [SetUp]
        public async Task Setup()
        {
            string connectionString = TestContext.Parameters["MongoDbUrl"];
            string databaseName = "test_yggdrasil";

            StorageMongoDbConfiguration configuration = new StorageMongoDbConfiguration()
            {
                DatabaseName = databaseName,
                ConnectionString = connectionString,
            };
            _storage = new MongoDbCampaignStorage(configuration);

            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);

            await _storage.Connect();
        }

        /// <summary>
        /// This tests that not only is the campaign records deleted, but all associated records are deleted as well
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task DeleteCampaignRemovesAllEntries()
        {
            string campaignId = await _storage.CreateCampaign("Me", "Test", "Description");
            string locationId = (await _storage.AddLocation(campaignId, "Location", "Test", null, null, Array.Empty<string>())).ID;

            await _storage.DeleteCampaign(campaignId);

            IMongoCollection<MongoLocation> collection = _database.GetCollection<MongoLocation>("locations");

            FilterDefinition<MongoLocation> locationsFilter = Builders<MongoLocation>.Filter
                .Eq(p => p.CampaignId, campaignId);

            long count = await collection.CountDocumentsAsync(locationsFilter, new CountOptions() { Limit = 1 });

            Assert.AreEqual(0, count);
        }
    }
}
