using MongoDB.Bson.Serialization.Attributes;
using Yggdrasil.Models.Locations;

namespace Yggdrasil.Server.Storage.Mongo
{
    /// <summary>
    /// Class for reading/writing location data from a MongoDB connection
    /// </summary>
    public class MongoLocation
    {
        /// <summary>
        /// Returns the communication model for this location
        /// </summary>
        /// <returns></returns>
        public Location ToLocation()
        {
            return new Location()
            {
                ID = Id,
                Name = Name,
                Population = Population,
                Description = Description,
                Tags = Tags,
            };
        }
        /// <summary>
        /// Gets or sets the ID of the location
        /// </summary>
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the ID of the parent
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// Gets or sets the ID of the campaign
        /// </summary>
        public string CampaignId { get; set; }
        /// <summary>
        /// Gets or sets the name of the location
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the population of this location
        /// </summary>
        public Population Population { get; set; }
        /// <summary>
        /// Gets or sets a description of this location in MarkDown
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets tags to use for this location
        /// </summary>
        public string[] Tags { get; set; }
    }
}
