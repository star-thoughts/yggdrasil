using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Models.Locations;

namespace Yggdrasil.Server.Services
{
    /// <summary>
    /// Base interface for managing locations in the system
    /// </summary>
    public interface ILocationsService
    {
        /// <summary>
        /// Gets the root locations for the given campaign
        /// </summary>
        /// <param name="campaignId">ID of the campaign to get the root locations for</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Details of the root locations in the campaign</returns>
        Task<LocationsList> GetRootLocations(string campaignId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets the given location
        /// </summary>
        /// <param name="campaignId">ID of the campaign containing the location</param>
        /// <param name="locationID">ID of the location</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Location data</returns>
        Task<Location> GetLocation(string campaignId, string locationId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Adds a new location to the campaign
        /// </summary>
        /// <param name="campaignId">ID of the campaign to add to</param>
        /// <param name="editingUser">The user that is adding this location</param>
        /// <param name="name">Name of the location to add</param>
        /// <param name="description">Description of the location</param>
        /// <param name="parent">The ID of the parent location</param>
        /// <param name="population">Population data for the location</param>
        /// <param name="tags">Tags to associate with the location</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>ID of the created location</returns>
        Task<string> AddLocation(string campaignId, string editingUser, string name, string? description, string? parentId, Population? population, string[]? tags, CancellationToken cancellationToken = default);
        /// <summary>
        /// Removes the given location
        /// </summary>
        /// <param name="campaignId">ID of the campaign that has the location</param>
        /// <param name="editingUser">User that is editing the campaign</param>
        /// <param name="locationId">ID of the location</param>
        /// <param name="childrenHandling">Whether or not to relocate children of this location to this location's parent</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        /// <exception cref="ArgumentNullException">A required parameter was not supplied</exception>
        Task RemoveLocation(string campaignId, string editingUser, string locationId, HandleChildren childrenHandling, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates the given location
        /// </summary>
        /// <param name="campaignId">ID of the campaign that contains the location to update</param>
        /// <param name="editingUser">User editing the campaign</param>
        /// <param name="locationId">ID of the location to update</param>
        /// <param name="name">New name for the location, or null to not update</param>
        /// <param name="description">New decription for the location, or null to not update</param>
        /// <param name="population">New population data for the location, or null to not update</param>
        /// <param name="tags">New tags for the location, or null to not update</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        /// <exception cref="ArgumentNullException">A required parameter was null or empty</exception>
        Task UpdateLocation(string campaignId, string editingUser, string locationId, string? name, string? description, Population? population, string[]? tags, CancellationToken cancellationToken = default);
    }
}