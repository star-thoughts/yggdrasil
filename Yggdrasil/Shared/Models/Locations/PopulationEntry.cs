namespace Yggdrasil.Models.Locations
{
    /// <summary>
    /// Contains an entry with information about a specific type of population in a location
    /// </summary>
    public sealed class PopulationEntry
    {
        /// <summary>
        /// Gets or sets the type this population refers to.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the number of this population type in the location.  Can be a number or percentage that ends with %.
        /// </summary>
        /// <remarks>
        /// This can be a specific number, or if ended with a % sign, is a percentage of total population.
        /// </remarks>
        public string Count { get; set; }
    }
}