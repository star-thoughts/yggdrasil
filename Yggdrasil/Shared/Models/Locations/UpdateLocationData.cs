namespace Yggdrasil.Models.Locations
{
    /// <summary>
    /// Contains properties to use to update a location
    /// </summary>
    public sealed class UpdateLocationData
    {
        /// <summary>
        /// Gets or sets the new name for the location, or null to not update the name
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets the new description for the location, or null to not update the description
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Gets or sets the new population data, or null to not update the population data
        /// </summary>
        public Population? Population { get; set; }
        /// <summary>
        /// Gets or sets the tags to use for the location, or null to not update the tags
        /// </summary>
        public string[]? Tags { get; set; }
    }
}
