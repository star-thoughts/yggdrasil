namespace Yggdrasil.Models.Locations
{
    /// <summary>
    /// Contains population data for a location
    /// </summary>
    public sealed class Population
    {
        /// <summary>
        /// Gets or sets a collection of entries containing different populations in the location
        /// </summary>
        public PopulationEntry[] Populations { get; set; }
    }
}