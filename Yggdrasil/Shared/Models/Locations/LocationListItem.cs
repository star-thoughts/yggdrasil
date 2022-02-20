namespace Yggdrasil.Models.Locations
{
    /// <summary>
    /// Item that contains shortened information for a list of locations
    /// </summary>
    public sealed class LocationListItem
    {
        /// <summary>
        /// Gets or sets the ID of this location
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// Gets or sets the name of this location
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Gets or sets the associated tags, or null if tags were not included in the list
        /// </summary>
        public string[]? Tags { get; set; }
    }
}