using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yggdrasil.Server.Configuration
{
    /// <summary>
    /// Configuration options for identity
    /// </summary>
    public sealed class IdentityConfiguration
    {
        /// <summary>
        /// Gets or sets the default account to use for administrator
        /// </summary>
        /// <remarks>
        /// This is only when creating the identity database.  Once the database exists the account can be changed/removed/etc
        /// </remarks>
        public string? AdminAccount { get; set; }
        /// <summary>
        /// Gets or sets hte default admin password
        /// </summary>
        /// <remarks>
        /// This is only when creating the identity database.  Once the database exists the account can be changed/removed/etc
        /// </remarks>
        public string? AdminPassword { get; set; }
        /// <summary>
        /// Configuration information for using MongoDB as an identity database
        /// </summary>
        public IdentityStorageConfiguration? MongoDB { get; set; }
    }
}
