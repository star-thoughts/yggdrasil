using System.Linq;
using Yggdrasil.Models.Locations;

namespace Yggdrasil.Server.Storage.Mongo
{
    /// <summary>
    /// Contains location information, including references to child and parent locations
    /// </summary>
    public class MongoLocationWithReferences
    {
        /// <summary>
        /// Converts the data in this location into a <see cref="Location"/> object
        /// </summary>
        /// <returns>Location object</returns>
        public Location ToLocation()
        {
            return new Location()
            {
                ID = Location.Id,
                Description = Location.Description,
                Name = Location.Name,
                Population = Location.Population,
                Tags = Location.Tags,
                Parent = Parent.ToLocationListItem(),
                ChildLocations = Children.Select(p => p.ToLocationListItem()).ToArray()
            };
        }

        /// <summary>
        /// Gets or sets the location
        /// </summary>
        public MongoLocation Location { get; set; }
        /// <summary>
        /// Gets or sets the parent reference
        /// </summary>
        public MongoLocationReference Parent { get; set; }
        /// <summary>
        /// Gets or sets child references
        /// </summary>
        public MongoLocationReference[] Children { get; set; }
    }
}
