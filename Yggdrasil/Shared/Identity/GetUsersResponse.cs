using System.Collections.Generic;

namespace Yggdrasil.Identity
{
    /// <summary>
    /// Response to a request to get users in the system
    /// </summary>
    public sealed class GetUsersResponse
    {
        /// <summary>
        /// Gets or sets a collection of users
        /// </summary>
        public IEnumerable<UserInfo> Users { get; set; }
    }
}
