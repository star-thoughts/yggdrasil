using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.Client.Services
{
    public interface ICampaignService
    {
        Task<CampaignList> GetCampaigns(CancellationToken cancellationToken = default);
        Task<string> CreateCampaign(string name, string shortDescription, CancellationToken cancellationToken = default);
        Task UpdateCampaign(string campaignID, string name, string shortDescription, CancellationToken cancellationToken = default);
        Task<OpenCampaignResult> OpenCampaign(string campaignID, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the campaign with the given ID
        /// </summary>
        /// <param name="campaignID">ID of the campaign to delete</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        Task DeleteCampaign(string campaignID, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the current campaign
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        Task DeleteCampaign(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets information on the open campaign
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Campaign data</returns>
        Task<GetCampaignResult> GetCampaign(CancellationToken cancellationToken = default);
        #region Characters
        /// <summary>
        /// Gets character information for a campaign
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Character information</returns>
        Task<CampaignPlayerCharacters> GetCharacters(CancellationToken cancellationToken = default);

        /// <summary>
        /// Claims the character with the given ID for the current user
        /// </summary>
        /// <param name="characterID">ID of the charater to claim</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        Task ClaimCharacter(string characterID, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets a list of users assigned to the campaign
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Collection of user data for users in the campaign</returns>
        Task<IEnumerable<CampaignUserData>> GetCampaignUsers(CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates player character data
        /// </summary>
        /// <param name="character">Player character to update</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        Task UpdatePlayerCharacter(CampaignPlayerCharacter character, CancellationToken cancellationToken = default);
        /// <summary>
        /// Creates a new player character
        /// </summary>
        /// <param name="character">Player character to create</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        Task CreatePlayerCharacter(CampaignPlayerCharacter character, CancellationToken cancellationToken = default);
        /// <summary>
        /// Removes a character from the campaign
        /// </summary>
        /// <param name="characterID">ID of the character to remove</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        Task RemoveCharacter(string characterID, CancellationToken cancellationToken = default);
        #endregion
    }
}