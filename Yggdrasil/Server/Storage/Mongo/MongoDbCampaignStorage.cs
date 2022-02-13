using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Auditing;
using Yggdrasil.Identity;
using Yggdrasil.Models;
using Yggdrasil.Models.Locations;
using Yggdrasil.Server.Configuration;

namespace Yggdrasil.Server.Storage.Mongo
{
    /// <summary>
    /// Storage using Mongo DB
    /// </summary>
    public class MongoDbCampaignStorage : ICampaignStorage
    {
        public MongoDbCampaignStorage(IOptions<StorageConfiguration> configuration)
        {
            _configuration = configuration?.Value.MongoDB;

            if (_configuration == null)
                throw new ArgumentNullException(nameof(configuration));
        }

        private readonly StorageMongoDbConfiguration _configuration;
        private static readonly UpdateOptions NotUpsertOptions = new UpdateOptions() { IsUpsert = false };
        private static readonly InsertOneOptions InsertOneOptions = new InsertOneOptions();
        private MongoClient _client;
        private IMongoDatabase _database;

        private const string AuditCollection = "audit";
        private const string CampaignsCollection = "campaigns";
        private const string LocationsCollection = "locations";

        public Task Connect(CancellationToken cancellationToken = default)
        {
            if (_client == null && _database == null)
            {
                _client = new MongoClient(_configuration.ConnectionString);
                _database = _client.GetDatabase(_configuration.DatabaseName);
            }
            return Task.CompletedTask;
        }
        #region Camapigns
        public async Task<IEnumerable<CampaignOverview>> GetCampaigns(CancellationToken cancellationToken = default)
        {
            IMongoCollection<MongoCampaign> campaignsCollection = GetDatabase().GetCollection<MongoCampaign>(CampaignsCollection);

            List<CampaignOverview> result = await campaignsCollection.Find(FilterDefinition<MongoCampaign>.Empty)
                .Project(p => new CampaignOverview()
                {
                    ID = p.ID.ToString(),
                    DungeonMaster = p.DungeonMaster,
                    ShortDescription = p.Overview,
                    Name = p.Name,
                })
                .ToListAsync(cancellationToken);

            return result;
        }
        /// <summary>
        /// Creates a new campaign
        /// </summary>
        /// <param name="dungeonMaster">Username of the account to assign as the Dungeon Master</param>
        /// <param name="name">Name of the campaign</param>
        /// <param name="shortDescription">Short description of the campaign</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>ID of the campaign</returns>
        public async Task<string> CreateCampaign(string dungeonMaster, string name, string shortDescription, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dungeonMaster))
                throw new ArgumentNullException(nameof(dungeonMaster));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            IMongoCollection<MongoCampaign> collection = GetDatabase().GetCollection<MongoCampaign>(CampaignsCollection);

            MongoCampaign campaign = new MongoCampaign()
            {
                Name = name,
                Overview = shortDescription,
                DungeonMaster = dungeonMaster,
                Users = new CampaignUserData[] { new CampaignUserData() { UserName = dungeonMaster, Roles = Roles.DungeonMaster } },
            };

            await collection.InsertOneAsync(campaign, InsertOneOptions, cancellationToken);

            return campaign.ID.ToString();
        }


        /// <summary>
        /// Gets the campaign with the given ID
        /// </summary>
        /// <param name="campaignID">ID of the campaign to get</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns></returns>
        public async Task<GetCampaignResult> GetCampaign(string campaignID, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignID))
                throw new ArgumentNullException(nameof(campaignID));

            if (!ObjectId.TryParse(campaignID, out ObjectId objectID))
                throw new ArgumentException("CampaignID must translate to an ObjectID", nameof(campaignID));

            IMongoCollection<MongoCampaign> collection = GetDatabase().GetCollection<MongoCampaign>(CampaignsCollection);

            FilterDefinition<MongoCampaign> filter = Builders<MongoCampaign>.Filter
                .Eq(p => p.ID, objectID);

            return await collection.Find(filter)
                .Limit(1)
                .Project(p => new GetCampaignResult()
                {
                    Name = p.Name,
                    Overview = p.Overview,
                    Users = p.Users,
                })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Deletes the campaign given by <paramref name="campaignID"/>
        /// </summary>
        /// <param name="campaignID">ID of the camapign to delete</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        public Task DeleteCampaign(string campaignID, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignID))
                throw new ArgumentNullException(nameof(campaignID));

            if (!ObjectId.TryParse(campaignID, out ObjectId objectID))
                throw new ArgumentException("CampaignID must translate to an ObjectID", nameof(campaignID));

            IMongoCollection<MongoCampaign> collection = GetDatabase().GetCollection<MongoCampaign>(CampaignsCollection);

            FilterDefinition<MongoCampaign> filter = Builders<MongoCampaign>.Filter
                .Eq(p => p.ID, objectID);

            return collection.DeleteOneAsync(filter);
        }
        #endregion
        #region Player Characters
        /// <summary>
        /// Gets the player characters in the given campaign
        /// </summary>
        /// <param name="campaignID">ID of the campaign</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Player character information</returns>
        public async Task<CampaignPlayerCharacters> GetPlayerCharacters(string campaignID, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignID))
                throw new ArgumentNullException(nameof(campaignID));

            if (!ObjectId.TryParse(campaignID, out ObjectId objectID))
                throw new ArgumentException("CampaignID must translate to an ObjectID", nameof(campaignID));

            IMongoCollection<MongoCampaign> collection = GetDatabase().GetCollection<MongoCampaign>(CampaignsCollection);

            FilterDefinition<MongoCampaign> filter = Builders<MongoCampaign>.Filter
                .Eq(p => p.ID, objectID);

            IEnumerable<CampaignPlayerCharacter> characters = await collection.Find(filter)
                .Limit(1)
                .Project(p => p.PlayerCharacters)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return new CampaignPlayerCharacters()
            {
                Characters = characters,
            };
        }
        /// <summary>
        /// Claims a player character for a specific player
        /// </summary>
        /// <param name="campaignID">ID of the campaign</param>
        /// <param name="characterID">ID of the player character</param>
        /// <param name="userName">Player's username</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task ClaimCharacter(string campaignID, string characterID, string userName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignID))
                throw new ArgumentNullException(nameof(campaignID));

            if (!ObjectId.TryParse(campaignID, out ObjectId objectID))
                throw new ArgumentException("CampaignID must translate to an ObjectID", nameof(campaignID));

            IMongoCollection<MongoCampaign> collection = GetDatabase().GetCollection<MongoCampaign>(CampaignsCollection);

            FilterDefinition<MongoCampaign> filter = Builders<MongoCampaign>.Filter.And(
                Builders<MongoCampaign>.Filter.Eq(p => p.ID, objectID),
                Builders<MongoCampaign>.Filter.ElemMatch(p => p.PlayerCharacters, p => p.ID == characterID));

#pragma warning disable CS0251 // Indexing an array with a negative index
            UpdateDefinition<MongoCampaign> update = Builders<MongoCampaign>.Update
                .Set(p => p.PlayerCharacters[-1].UserName, userName);
#pragma warning restore CS0251 // Indexing an array with a negative index

            await collection.UpdateOneAsync(filter, update, NotUpsertOptions, cancellationToken);
        }

        /// <summary>
        /// Gets the player character in the given campaign with the given ID
        /// </summary>
        /// <param name="campaignID">ID of the campaign</param>
        /// <param name="characterID">ID of the charatcer to get</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Player character information</returns>
        public async Task<CampaignPlayerCharacter> GetPlayerCharacter(string campaignID, string characterID, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignID))
                throw new ArgumentNullException(nameof(campaignID));

            if (!ObjectId.TryParse(campaignID, out ObjectId objectID))
                throw new ArgumentException("CampaignID must translate to an ObjectID", nameof(campaignID));

            IMongoCollection<MongoCampaign> collection = GetDatabase().GetCollection<MongoCampaign>(CampaignsCollection);

            FilterDefinition<MongoCampaign> filter = Builders<MongoCampaign>.Filter.And(
                Builders<MongoCampaign>.Filter.Eq(p => p.ID, objectID),
                Builders<MongoCampaign>.Filter.ElemMatch(p => p.PlayerCharacters, p => p.ID == characterID));

#pragma warning disable CS0251 // Indexing an array with a negative index
            return await collection.Find(filter)
                .Limit(1)
                .Project(p => p.PlayerCharacters[-1])
                .FirstOrDefaultAsync(cancellationToken);
#pragma warning restore CS0251 // Indexing an array with a negative index
        }


        /// <summary>
        /// Gets a list of users for a campaign
        /// </summary>
        /// <param name="campaignID">ID of the campaign</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Collection of data with campaign users</returns>
        public async Task<IEnumerable<CampaignUserData>> GetCampaignUsers(string campaignID, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignID))
                throw new ArgumentNullException(nameof(campaignID));

            if (!ObjectId.TryParse(campaignID, out ObjectId objectID))
                throw new ArgumentException("CampaignID must translate to an ObjectID", nameof(campaignID));

            IMongoCollection<MongoCampaign> collection = GetDatabase().GetCollection<MongoCampaign>(CampaignsCollection);

            FilterDefinition<MongoCampaign> filter = Builders<MongoCampaign>.Filter
                .Eq(p => p.ID, objectID);

            return await collection.Find(filter)
                .Limit(1)
                .Project(p => p.Users)
                .FirstOrDefaultAsync(cancellationToken);
        }
        /// <summary>
        /// Updates a player character's information
        /// </summary>
        /// <param name="campaignID">ID of the campaign</param>
        /// <param name="character">Character to update</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task UpdatePlayerCharacter(string campaignID, CampaignPlayerCharacter character, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignID))
                throw new ArgumentNullException(nameof(campaignID));
            if (character == null)
                throw new ArgumentNullException(nameof(character));
            if (string.IsNullOrWhiteSpace(character.ID))
                throw new ItemNotFoundException(ItemType.PlayerCharacter, string.Empty);

            if (!ObjectId.TryParse(campaignID, out ObjectId objectID))
                throw new ArgumentException("CampaignID must translate to an ObjectID", nameof(campaignID));

            IMongoCollection<MongoCampaign> collection = GetDatabase().GetCollection<MongoCampaign>(CampaignsCollection);

            FilterDefinition<MongoCampaign> filter = Builders<MongoCampaign>.Filter.And(
                Builders<MongoCampaign>.Filter.Eq(p => p.ID, objectID),
                Builders<MongoCampaign>.Filter.ElemMatch(p => p.PlayerCharacters, p => p.ID == character.ID));

#pragma warning disable CS0251 // Indexing an array with a negative index
            UpdateDefinition<MongoCampaign> update = Builders<MongoCampaign>.Update
                .Set(p => p.PlayerCharacters[-1], character);
#pragma warning restore CS0251 // Indexing an array with a negative index

            UpdateResult result = await collection.UpdateOneAsync(filter, update, NotUpsertOptions, cancellationToken);

            if (result.MatchedCount == 0)
                throw new ItemNotFoundException(ItemType.PlayerCharacter, character.ID);
        }
        /// <summary>
        /// Creates new player character's information
        /// </summary>
        /// <param name="campaignID">ID of the campaign</param>
        /// <param name="character">Character to create</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>ID of the character created</returns>
        public async Task<string> CreatePlayerCharacter(string campaignID, CampaignPlayerCharacter character, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignID))
                throw new ArgumentNullException(nameof(campaignID));
            if (character == null)
                throw new ArgumentNullException(nameof(character));

            if (!ObjectId.TryParse(campaignID, out ObjectId objectID))
                throw new ArgumentException("CampaignID must translate to an ObjectID", nameof(campaignID));

            character.ID = Guid.NewGuid().ToString();

            IMongoCollection<MongoCampaign> collection = GetDatabase().GetCollection<MongoCampaign>(CampaignsCollection);

            FilterDefinition<MongoCampaign> filter = Builders<MongoCampaign>.Filter
                   .Eq(p => p.ID, objectID);

            UpdateDefinition<MongoCampaign> update = Builders<MongoCampaign>.Update
                .Push(p => p.PlayerCharacters, character);

            await collection.UpdateOneAsync(filter, update, NotUpsertOptions, cancellationToken);

            return character.ID;
        }
        /// <summary>
        /// Removes a character from a campaign
        /// </summary>
        /// <param name="campaignID">ID of the campaign the character is in</param>
        /// <param name="characterID">ID of the character to remove</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task RemoveCharacter(string campaignID, string characterID, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignID))
                throw new ArgumentNullException(nameof(campaignID));

            if (!ObjectId.TryParse(campaignID, out ObjectId objectID))
                throw new ArgumentException("CampaignID must translate to an ObjectID", nameof(campaignID));

            IMongoCollection<MongoCampaign> collection = GetDatabase().GetCollection<MongoCampaign>(CampaignsCollection);

            FilterDefinition<MongoCampaign> filter = Builders<MongoCampaign>.Filter.Eq(p => p.ID, objectID);

            UpdateDefinition<MongoCampaign> update = Builders<MongoCampaign>.Update
                .PullFilter(p => p.PlayerCharacters, p => p.ID == characterID);

            await collection.UpdateOneAsync(filter, update, NotUpsertOptions, cancellationToken);
        }

        #endregion
        #region Locations
        /// <summary>
        /// Adds a new location to the campaign
        /// </summary>
        /// <param name="campaigID">ID of the campaign to add to</param>
        /// <param name="name">Name of the location to add</param>
        /// <param name="description">Description of the location</param>
        /// <param name="parent">The ID of the parent location</param>
        /// <param name="population">Population data for the location</param>
        /// <param name="tags">Tags to associate with the location</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns></returns>
        public async Task<string> AddLocation(string campaigID, string name, string description, string parentId, Population population, string[] tags, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaigID))
                throw new ArgumentNullException(nameof(campaigID));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            IMongoCollection<MongoLocation> collection = GetDatabase().GetCollection<MongoLocation>(LocationsCollection);

            MongoLocation location = new MongoLocation()
            {
                Description = description,
                Population = population,
                Tags = tags,
                Name = name,
                ParentId = parentId,
            };

            await collection.InsertOneAsync(location, InsertOneOptions, cancellationToken);

            return location.Id;
        }

        /// <summary>
        /// Updates location information
        /// </summary>
        /// <param name="campaignId">ID of the campaign containing the location</param>
        /// <param name="locationID">ID of the location to update</param>
        /// <param name="name">New name for the location, or null to not update it</param>
        /// <param name="description">New description for the location, or null to not update it</param>
        /// <param name="population">New population for the location, or null not to update it</param>
        /// <param name="tags">New tags for the location, or null not to update it</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Updated location data</returns>
        public async Task<Location> UpdateLocation(string campaignId, string locationID, string name, string description, Population population, string[] tags, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignId))
                throw new ArgumentNullException(nameof(campaignId));
            if (string.IsNullOrWhiteSpace(locationID))
                throw new ArgumentNullException(nameof(locationID));

            IMongoCollection<MongoLocation> collection = GetDatabase().GetCollection<MongoLocation>(LocationsCollection);

            FilterDefinition<MongoLocation> filter = Builders<MongoLocation>.Filter.And(
                Builders<MongoLocation>.Filter.Eq(p => p.Id, locationID),
                Builders<MongoLocation>.Filter.Eq(p => p.CampaignId, campaignId));

            //  Only updating the campaign ID just to have an update definition base to build on
            UpdateDefinition<MongoLocation> update = Builders<MongoLocation>.Update
                .Set(p => p.CampaignId, campaignId);

            if (name is not null)
                update = update.Set(p => p.Name, name);
            if (description is not null)
                update = update.Set(p => p.Description, description);
            if (population is not null)
                update = update.Set(p => p.Population, population);
            if (tags is not null)
                update = update.Set(p => p.Tags, tags);

            MongoLocation location = await collection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<MongoLocation, MongoLocation>() { ReturnDocument = ReturnDocument.After });

            return location.ToLocation();
        }

        /// <summary>
        /// Gets the given location
        /// </summary>
        /// <param name="campaignId">ID of the campaign containing the location</param>
        /// <param name="locationID">ID of the location</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Location data</returns>
        public async Task<Location> GetLocation(string campaignId, string locationID, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignId))
                throw new ArgumentNullException(nameof(campaignId));
            if (string.IsNullOrWhiteSpace(locationID))
                throw new ArgumentNullException(nameof(locationID));

            IMongoCollection<MongoLocation> collection = GetDatabase().GetCollection<MongoLocation>(LocationsCollection);

            FilterDefinition<MongoLocation> filter = Builders<MongoLocation>.Filter.And(
                Builders<MongoLocation>.Filter.Eq(p => p.Id, locationID),
                Builders<MongoLocation>.Filter.Eq(p => p.CampaignId, campaignId));

            string parentID = $"Location.{nameof(MongoLocation.ParentId)}";

            MongoLocationWithReferences result = await collection.Aggregate()
                .Match(p => p.CampaignId == campaignId && p.Id == locationID)
                .Project(p => new MongoLocationWithReferences() { Location = p })
                .Lookup(nameof(LocationsCollection), parentID, nameof(MongoLocation.Id), nameof(MongoLocationWithReferences.Parent))
                .Unwind(nameof(MongoLocationWithReferences.Parent))
                .Lookup(nameof(LocationsCollection), "Location._id", nameof(MongoLocation.ParentId), nameof(MongoLocationWithReferences.Children))
                .As<MongoLocationWithReferences>()
                .FirstOrDefaultAsync();

            return result.ToLocation();
        }
        #endregion
        #region Audit
        public Task AddAuditRecord(AuditRecord auditRecord, CancellationToken cancellationToken = default)
        {
            IMongoCollection<MongoAuditRecord> collection = GetDatabase().GetCollection<MongoAuditRecord>(AuditCollection);

            MongoAuditRecord record = MongoAuditRecord.FromAuditRecord(auditRecord);
            return collection.InsertOneAsync(record, InsertOneOptions, cancellationToken);
        }
        #endregion
        #region Helpers
        private IMongoDatabase GetDatabase()
        {
            if (_database == null)
                throw new InvalidOperationException("The database must be connected to before it can be used.");

            return _database;
        }
        #endregion
    }
}
