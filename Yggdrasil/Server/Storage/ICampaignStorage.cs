using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.Server.Storage
{
    /// <summary>
    /// Interface for communicating with backend storage
    /// </summary>
    public interface ICampaignStorage : IAuditStorage
    {
        #region Connection
        /// <summary>
        /// Connects to the data store
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling hte operation</param>
        /// <returns>Task for asynchronous completion</returns>
        Task Connect(CancellationToken cancellationToken = default);
        #endregion
        #region Campaigns
        /// <summary>
        /// Gets a collection of all campaigns in the system
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Collection of campaign information</returns>
        Task<IEnumerable<CampaignOverview>> GetCampaigns(CancellationToken cancellationToken = default);
        /// <summary>
        /// Creates a new campaign
        /// </summary>
        /// <param name="dungeonMaster">Username of the account to assign as the Dungeon Master</param>
        /// <param name="name">Name of the campaign</param>
        /// <param name="shortDescription">Short description of the campaign</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>ID of the campaign</returns>
        Task<string> CreateCampaign(string dungeonMaster, string name, string shortDescription, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the campaign given by <paramref name="campaignID"/>
        /// </summary>
        /// <param name="campaignID">ID of the camapign to delete</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        Task DeleteCampaign(string campaignID, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets the campaign with the given ID
        /// </summary>
        /// <param name="campaignID">ID of the campaign to get</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns></returns>
        Task<GetCampaignResult> GetCampaign(string campaignID, CancellationToken cancellationToken = default);
        #endregion
        #region Players/Player Characters
        /// <summary>
        /// Gets the player characters in the given campaign
        /// </summary>
        /// <param name="campaignID">ID of the campaign</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Player character information</returns>
        Task<CampaignPlayerCharacters> GetPlayerCharacters(string campaignID, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets the player character in the given campaign with the given ID
        /// </summary>
        /// <param name="campaignID">ID of the campaign</param>
        /// <param name="characterID">ID of the charatcer to get</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Player character information</returns>
        Task<CampaignPlayerCharacter> GetPlayerCharacter(string campaignID, string characterID, CancellationToken cancellationToken = default);
        /// <summary>
        /// Claims a player character for a specific player
        /// </summary>
        /// <param name="campaignID">ID of the campaign</param>
        /// <param name="characterID">ID of the player character</param>
        /// <param name="userName">Player's username</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        Task ClaimCharacter(string campaignID, string characterID, string userName, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets a list of users for a campaign
        /// </summary>
        /// <param name="campaignID">ID of the campaign</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Collection of data with campaign users</returns>
        Task<IEnumerable<CampaignUserData>> GetCampaignUsers(string campaignID, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates a player character's information
        /// </summary>
        /// <param name="campaignID">ID of the campaign</param>
        /// <param name="character">Character to update</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        Task UpdatePlayerCharacter(string campaignID, CampaignPlayerCharacter character, CancellationToken cancellationToken = default);
        /// <summary>
        /// Creates new player character's information
        /// </summary>
        /// <param name="campaignID">ID of the campaign</param>
        /// <param name="character">Character to create</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>ID of the character created</returns>
        Task<string> CreatePlayerCharacter(string campaignID, CampaignPlayerCharacter character, CancellationToken cancellationToken = default);
        /// <summary>
        /// Removes a character from a campaign
        /// </summary>
        /// <param name="campaignID">ID of the campaign the character is in</param>
        /// <param name="characterID">ID of the character to remove</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        Task RemoveCharacter(string campaignID, string characterID, CancellationToken cancellationToken = default);
        #endregion
    }
}
