﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Models;
using Yggdrasil.Models.Locations;

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
        #region Locations
        /// <summary>
        /// Gets the root locations for the given campaign
        /// </summary>
        /// <param name="campaignId">ID of the campaign to get the root location for</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Collection of locations that are the root locations for the campaign</returns>
        Task<IEnumerable<Location>> GetRootLocations(string campaignId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Adds a new location to the campaign
        /// </summary>
        /// <param name="campaignId">ID of the campaign to add to</param>
        /// <param name="name">Name of the location to add</param>
        /// <param name="description">Description of the location</param>
        /// <param name="parent">The ID of the parent location</param>
        /// <param name="population">Population data for the location</param>
        /// <param name="tags">Tags to associate with the location</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>ID of the created location</returns>
        Task<string> AddLocation(string campaignId, string name, string description, string parentId, Population population, string[] tags, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the given location from the database
        /// </summary>
        /// <param name="campaignId">ID of the campaign to remove</param>
        /// <param name="locationId">ID of the location to remove</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        /// <exception cref="ArgumentNullException">No campaign or location ID was specified</exception>
        Task RemoveLocation(string campaignId, string locationId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates location information
        /// </summary>
        /// <param name="campaignId">ID of the campaign containing the location</param>
        /// <param name="locationID">ID of the location to update</param>
        /// <param name="name">New name for the location, or null to not update it</param>
        /// <param name="description">New description for the location, or null to not update it</param>
        /// <param name="population">New population for the location, or null not to update it</param>
        /// <param name="tags">New tags for the location, or null not to update it</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Updated location data</returns>
        Task<Location> UpdateLocation(string campaignId, string locationID, string name, string description, Population population, string[] tags, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets the given location
        /// </summary>
        /// <param name="campaignId">ID of the campaign containing the location</param>
        /// <param name="locationID">ID of the location</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Location data</returns>
        Task<Location> GetLocation(string campaignId, string locationID, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes the given location
        /// </summary>
        /// <param name="campaignId">ID of the campaign containing the location</param>
        /// <param name="locationId">ID of the location</param>
        /// <param name="relocateChildren">Whether or not to relocate the children to this location's parent</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Collection of locations that were updated.  Does not include deleted location.</returns>
        /// <remarks>
        /// When a location is deleted, if <paramref name="relocateChildren"/> is true, then the child locations parent's are updated
        /// first to point to the parent of the location given in <paramref name="locationId"/>.  If <paramref name="relocateChildren"/> is false, then the children
        /// are "abandoned" and become root locations.
        /// </remarks>
        Task<IEnumerable<Location>> DeleteLocation(string campaignId, string locationId, bool relocateChildren, CancellationToken cancellationToken = default);
        /// <summary>
        /// Moves locations to a new parent, or to root
        /// </summary>
        /// <param name="campaignId">ID of the campaign containing the locations to move</param>
        /// <param name="newParentId">New parent ID of the locations, or null if the locations are to be root locations</param>
        /// <param name="locationIds">Collection of IDs of locations to move</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns></returns>
        Task<IEnumerable<Location>> MoveLocations(string campaignId, string newParentId, IEnumerable<string> locationIds, CancellationToken cancellationToken = default);
        #endregion
    }
}
