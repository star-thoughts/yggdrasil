using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yggdrasil.Server.Configuration
{
    /// <summary>
    /// Configuration options for JWT tokens
    /// </summary>
    public sealed class JwtConfiguration
    {
        /// <summary>
        /// Gets or sets the key-phrase to use for signing JWTs
        /// </summary>
        public string? Key { get; set; }
        /// <summary>
        /// Gets or sets the specified issuer of a JWT
        /// </summary>
        public string? Issuer { get; set; }
        /// <summary>
        /// Gets or sets the number of days before a token expires
        /// </summary>
        public int ExpireDays { get; set; }
    }
}
