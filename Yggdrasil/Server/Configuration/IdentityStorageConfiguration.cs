namespace Yggdrasil.Server.Configuration
{
    /// <summary>
    /// Configuration information for using MongoDB as an identity database
    /// </summary>
    public sealed class IdentityStorageConfiguration
    {
        /// <summary>
        /// Gets or sets the URI to use to connect to the mongo DB
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Gets or sets the name of the database to use
        /// </summary>
        public string Database { get; set; }
    }
}