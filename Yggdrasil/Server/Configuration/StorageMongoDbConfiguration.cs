namespace Yggdrasil.Server.Configuration
{
    /// <summary>
    /// Configuration data for MongoDB
    /// </summary>
    public record StorageMongoDbConfiguration
    {
        /// <summary>
        /// Gets the URI for storing campaign data
        /// </summary>
        public string? ConnectionString { get; init; }
        /// <summary>
        /// Gets the database to use for storage
        /// </summary>
        public string? DatabaseName { get; init; }
    }
}