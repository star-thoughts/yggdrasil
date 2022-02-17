using MongoDB.Bson.Serialization.Attributes;
using Yggdrasil.Models.Locations;

namespace Yggdrasil.Server.Storage.Mongo
{
    /// <summary>
    /// Contains a location reference
    /// </summary>
    public class MongoLocationReference
    {
        /// <summary>
        /// Creates a location list item from this location data
        /// </summary>
        /// <returns>Location list item</returns>
        public LocationListItem ToLocationListItem()
        {
            return new LocationListItem()
            {
                Id = Id,
                Name = Name,
                Tags = Tags,
            };
        }
        /// <summary>
        /// Gets or sets the ID of the referenced location
        /// </summary>
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the referenced location
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets tags associated with the referenced location
        /// </summary>
        public string[]? Tags { get; set; }
    }
}