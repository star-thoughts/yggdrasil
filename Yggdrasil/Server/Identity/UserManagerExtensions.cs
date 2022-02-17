using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Yggdrasil.Server.Identity
{
    static class UserManagerExtensions
    {
        /// <summary>
        /// Gets the user information for sending to a client
        /// </summary>
        /// <param name="user">User to create user information for</param>
        /// <param name="userManager">User manager to use to get information</param>
        /// <returns>Client-friendly user information</returns>
        //public static async Task<UserInfo> ToUserInfo(this ApplicationUser user, UserManager<ApplicationUser> userManager)
        //{
        //    if (user == null)
        //        throw new ArgumentNullException(nameof(user));
        //    if (userManager == null)
        //        throw new ArgumentNullException(nameof(userManager));

        //    System.Collections.Generic.List<string> roles = (await userManager.GetRolesAsync(user)).ToList();

        //    return new UserInfo()
        //    {
        //        IsVerified = user.IsVerified,
        //        Roles = roles,
        //        UserName = user.UserName,
        //        IsLocked = await userManager.IsLockedOutAsync(user),
        //    };
        //}

        /// <summary>
        /// Gets the user with the given username
        /// </summary>
        /// <param name="userManager">Usermanager to get user from</param>
        /// <param name="userName">Name of the user to get</param>
        /// <returns>User identity information</returns>
        public static ApplicationUser? GetUser(this UserManager<ApplicationUser> userManager, string userName)
        {
            return userManager.Users.FirstOrDefault(p => p.NormalizedUserName == userName.ToUpperInvariant());
        }
    }
}
