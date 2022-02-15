namespace Yggdrasil.Models.Locations
{
    /// <summary>
    /// Contains information about a location
    /// </summary>
    public sealed class Location
    {
        /// <summary>
        /// Gets or sets the ID of the location
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Gets or sets the name of the location
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the ID of the parent
        /// </summary>
        public string ParentId { get; set; }
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
        /// <summary>
        /// Gets or sets the parent location.  If null, then no parent information was included, but still may have a parent.
        /// </summary>
        public LocationListItem Parent { get; set; }
        /// <summary>
        /// Gets or sets immediate children locations.  If null or empty, no child information was included, but maystill have children.
        /// </summary>
        public LocationListItem[] ChildLocations { get; set; }
    }
}
