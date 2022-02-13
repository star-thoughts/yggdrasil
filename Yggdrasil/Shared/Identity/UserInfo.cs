using System.Collections.Generic;

namespace Yggdrasil.Identity
{
    /// <summary>
    /// Information about a user
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Gets or sets the name of the user
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets a collection of roles the user belongs to
        /// </summary>
        public List<string> Roles { get; set; }
        /// <summary>
        /// Gets or sets whether or not the user has been verified
        /// </summary>
        public bool IsVerified { get; set; }
        /// <summary>
        /// Gets or sets whether or not the user account is locked
        /// </summary>
        public bool IsLocked { get; set; }
    }
}
