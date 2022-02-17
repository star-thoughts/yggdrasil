using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yggdrasil.Server.Configuration
{
    /// <summary>
    /// Contains information about storing campaign data
    /// </summary>
    public sealed class StorageConfiguration
    {
        /// <summary>
        /// Gets or sets the MongoDB configuration for storing data
        /// </summary>
        public StorageMongoDbConfiguration? MongoDB { get; set; }
    }
}
