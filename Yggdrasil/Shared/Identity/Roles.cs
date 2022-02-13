using System.Collections.Generic;

namespace Yggdrasil.Identity
{
    /// <summary>
    /// Definitions for roles in the program
    /// </summary>
    public static class Roles
    {
        public const string CampaignRoles = DungeonMaster + "," + Player;
        #region Campaign Roles
        public const string CreateCampaign = nameof(CreateCampaign);
        public const string DungeonMaster = nameof(DungeonMaster);
        public const string Player = nameof(Player);
        #endregion
        #region Admin Roles
        public const string ManageUsers = nameof(ManageUsers);
        public const string ManageUserPermissions = nameof(ManageUserPermissions);
        public const string ManageCampaigns = nameof(ManageCampaigns);
        #endregion
        #region Combined Roles
        /// <summary>
        /// Anyone who can manage all campaigns, or is the current campaign's dungeon master can delete this campaign.  This should only be applied
        /// if the current campaign is the one being deleted.  Otherwise <see cref="ManageCampaigns"/> should be used for any campaign.
        /// </summary>
        public const string DeleteCampaign = nameof(DungeonMaster) + "," + nameof(ManageCampaigns);
        #endregion
        #region Helpers
        /// <summary>
        /// Gets all of the roles in the system
        /// </summary>
        /// <returns>Collection of roles in the system</returns>
        public static IEnumerable<string> GetAllRoles()
        {
            yield return CreateCampaign;
            yield return ManageUsers;
            yield return ManageUserPermissions;
            yield return DungeonMaster;
            yield return Player;
            yield return ManageCampaigns;
        }
        /// <summary>
        /// Gets the default roles applied to an administrator
        /// </summary>
        /// <returns>Collection of roles for the site administrator</returns>
        public static IEnumerable<string> GetDefaultAdminRoles()
        {
            yield return CreateCampaign;
            yield return ManageUsers;
            yield return ManageUserPermissions;
            yield return ManageCampaigns;
        }
        #endregion
    }
}
