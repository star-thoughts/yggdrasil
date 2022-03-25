using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Models.Locations;
using Yggdrasil.Server.Hubs;
using Yggdrasil.Server.Storage;

namespace Yggdrasil.Server.Services
{
    /// <summary>
    /// Service for managing locations
    /// </summary>
    public class LocationsService : ILocationsService
    {
        /// <summary>
        /// Constructs a new <see cref="LocationsService"/>
        /// </summary>
        /// <param name="storage">Interface to use for getting/storing data</param>
        /// <param name="hub">Hub for triggering events on clients</param>
        /// <param name="auditor">Interface to use for auditing</param>
        /// <exception cref="ArgumentNullException">A parameter was null</exception>
        public LocationsService(ICampaignStorage storage, IHubContext<ServiceHub>? hub, IAuditStorage auditor)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _hub = hub;
            _auditor = auditor ?? throw new ArgumentNullException(nameof(auditor));
        }

        private readonly ICampaignStorage _storage;
        private readonly IHubContext<ServiceHub>? _hub;
        private readonly IAuditStorage _auditor;

        /// <summary>
        /// Gets the root locations for the given campaign
        /// </summary>
        /// <param name="campaignId">ID of the campaign to get the root locations for</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Details of the root locations in the campaign</returns>
        public async Task<LocationsList> GetRootLocations(string campaignId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignId))
                throw new ArgumentNullException(nameof(campaignId));

            return new LocationsList()
            {
                Locations = (await _storage.GetRootLocations(campaignId, cancellationToken)).ToArray(),
            };
        }

        /// <summary>
        /// Gets the given location
        /// </summary>
        /// <param name="campaignId">ID of the campaign containing the location</param>
        /// <param name="locationID">ID of the location</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Location data</returns>
        public async Task<Location> GetLocation(string campaignId, string locationId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(nameof(campaignId)))
                throw new ArgumentNullException(nameof(campaignId));

            if (string.IsNullOrWhiteSpace(nameof(locationId)))
                throw new ArgumentNullException(nameof(locationId));

            return await _storage.GetLocation(campaignId, locationId, cancellationToken);
        }

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
        public async Task<string> AddLocation(string campaignId, string editingUser, string name, string? description, string? parentId, Population? population, string[]? tags, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(campaignId))
                throw new ArgumentNullException(nameof(campaignId));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Location location = await _storage.AddLocation(campaignId, name, description, parentId, population, tags ?? Array.Empty<string>(), cancellationToken);

            if (_hub != null)
                _ = _hub.LocationAdded(campaignId, editingUser, location);

            return location.ID ?? string.Empty;
        }

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
        public async Task RemoveLocation(string campaignId, string editingUser, string locationId, HandleChildren childrenHandling, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(campaignId))
                throw new ArgumentNullException(nameof(campaignId));
            if (!string.IsNullOrWhiteSpace(locationId))
                throw new ArgumentNullException(nameof(locationId));

            await _storage.RemoveLocation(campaignId, locationId, childrenHandling, cancellationToken);

            if (_hub != null)
                _ = _hub.LocationRemoved(campaignId, editingUser, locationId);
        }

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
        public async Task UpdateLocation(string campaignId, string editingUser, string locationId, string? name, string? description, Population? population, string[]? tags, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(campaignId))
                throw new ArgumentNullException(nameof(campaignId));
            if (!string.IsNullOrWhiteSpace(locationId))
                throw new ArgumentNullException(nameof(locationId));

            Location location = await _storage.UpdateLocation(campaignId, locationId, name, description, population, tags, cancellationToken);

            if (_hub != null)
                _ = _hub.LocationUpdated(campaignId, editingUser, location);
        }
    }
}
