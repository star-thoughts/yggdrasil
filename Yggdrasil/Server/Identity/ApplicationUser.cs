using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Identity;

namespace Yggdrasil.Server.Identity
{
    /// <summary>
    /// Application users's record
    /// </summary>
    public class ApplicationUser : MongoUser
    {
        /// <summary>
        /// Gets whether or not the user has been verified
        /// </summary>
        public bool IsVerified { get; set; }
        /// <summary>
        /// Gets or sets whether or not the user was previously locked out
        /// </summary>
        public bool WasLockedOut { get; set; }
        /// <summary>
        /// Gets or sets whether or not the user is the site administrator
        /// </summary>
        public bool IsSiteAdmin { get; set; }

        /// <summary>
        /// Converts this into a <see cref="UserInfo"/>
        /// </summary>
        /// <returns>User info with information about the user</returns>
        public async Task<UserInfo> ToUserInfo(UserManager<ApplicationUser> userManager)
        {
            if (userManager == null)
                throw new ArgumentNullException(nameof(userManager));

            List<string> roles = (await userManager.GetRolesAsync(this)).ToList();

            return new UserInfo()
            {
                IsLocked = await userManager.IsLockedOutAsync(this),
                IsVerified = IsVerified,
                Roles = roles,
                UserName = UserName,
            };
        }
    }
}
