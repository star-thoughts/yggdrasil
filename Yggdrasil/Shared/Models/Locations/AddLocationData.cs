namespace Yggdrasil.Models.Locations
{
    /// <summary>
    /// Conatins information about a location being added to a campaign
    /// </summary>
    public sealed class AddLocationData
    {
        /// <summary>
        /// Gets or sets the name to use for the new location
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description of the new location
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the ID of the parent of the new location, or null if it is a root location in the campaign
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// Gets or sets the population data for this location
        /// </summary>
        public Population Population { get; set; }
        /// <summary>
        /// Gets or sets tags to associate with this campaign
        /// </summary>
        public string[] Tags { get; set; }
    }
}
