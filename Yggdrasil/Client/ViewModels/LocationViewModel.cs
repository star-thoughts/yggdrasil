using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Client.Services;
using Yggdrasil.Models.Locations;

namespace Yggdrasil.Client.ViewModels
{
    public sealed class LocationViewModel
    {
        /// <summary>
        /// Constructs a new <see cref="LocationViewModel"/>
        /// </summary>
        /// <param name="location">Location this view model is for</param>
        /// <param name="service">Campaign service for sending updates to the server</param>
        public LocationViewModel(Location location, ICampaignService service)
        {
            _location = location ?? throw new ArgumentNullException(nameof(location));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        private Location _location;
        private readonly ICampaignService _service;
        private bool _dirty;

        /// <summary>
        /// Gets the ancestors for this
        /// </summary>
        public IEnumerable<LocationListItem> Ancestors { get => _location.ParentsPath; }
        /// <summary>
        /// Gets whether or not this view model has unsaved changes
        /// </summary>
        public bool IsDirty { get => _dirty; }
        /// <summary>
        /// Gets the ID of the location
        /// </summary>
        public string ID { get => _location.ID; }
        /// <summary>
        /// Gets the name of the location
        /// </summary>
        public string Name
        {
            get => _location.Name;
            set
            {
                _location.Name = value;
                _dirty = true;
            }
        }
        /// <summary>
        /// Gets or sets the ID of the location
        /// </summary>
        public string Id
        {
            get => _location.ID;
            set
            {
                _location.ID = value;
                _dirty = true;
            }
        }
        /// <summary>
        /// Gets or sets the population of this location
        /// </summary>
        public Population Population
        {
            get => _location.Population;
            set
            {
                _location.Population = value;
                _dirty = true;
            }
        }
        /// <summary>
        /// Gets or sets a description of this location in MarkDown
        /// </summary>
        public string Description
        {
            get => _location.Description;
            set
            {
                _location.Description = value;
                _dirty = true;
            }
        }
        /// <summary>
        /// Gets or sets tags to use for this location
        /// </summary>
        public string[] Tags
        {
            get => _location.Tags;
            set
            {
                _location.Tags = value;
                _dirty = true;
            }
        }

        /// <summary>
        /// Gets whether or not the details of this location are valid to be saved
        /// </summary>
        public bool IsValid
        {
            get => !string.IsNullOrWhiteSpace(Name);
        }
        /// <summary>
        /// Gets or sets immediate children locations.  If null or empty, no child information was included, but maystill have children.
        /// </summary>
        public IEnumerable<LocationListItem> ChildLocations { get => _location.ChildLocations; }

        /// <summary>
        /// Creates a view model based on the location ID
        /// </summary>
        /// <param name="locationID">ID of the location to get a view model for</param>
        /// <param name="service">Service for communication with the server</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>View model representing the location</returns>
        public static async Task<LocationViewModel> Create(string locationID, ICampaignService service, CancellationToken cancellationToken = default)
        {
            Location location = await service.GetLocation(locationID, cancellationToken);
            return new LocationViewModel(location, service);
        }

        public async Task Save(CancellationToken cancellationToken = default)
        {
            if (IsValid)
            {
                if (string.IsNullOrWhiteSpace(_location.ID))
                {
                    _location.ID = await _service.CreateLocation(_location.Name, _location.ParentId, _location.Description, _location.Population, _location.Tags, cancellationToken);
                }
                else
                {
                    if (IsDirty)
                        await _service.UpdateLocation(_location.ID, _location.Name, _location.Description, _location.Population, _location.Tags, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Attempts to update the location data for this view
        /// </summary>
        /// <param name="location">Location to update to</param>
        /// <returns>Whether or not the update was successful</returns>
        public bool TryUpdate(Location location)
        {
            if (string.Equals(location.ID, _location.ID))
            {
                _location = location;
                _dirty = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to update the location data for this view
        /// </summary>
        /// <param name="locations">Locations that were updated</param>
        /// <param name="cancellationToken">Tokenf or cancelling the operations</param>
        /// <returns>Whether or not the update was successful</returns>
        public async Task<bool> TryUpdate(LocationsMoved locations, CancellationToken cancellationToken = default)
        {
            bool updated = false;
            if (locations.Locations.TryGetValue(Id, out string locationId))
            {
                Location updatedParent = await _service.GetLocation(locationId, cancellationToken);
                if (updatedParent != null)
                {
                    _location.ParentsPath = new LocationListItem[] { new LocationListItem() { Id = updatedParent.ID, Name = updatedParent.Name, Tags = updatedParent.Tags } }.Concat(updatedParent.ParentsPath).ToArray();
                    _location.ParentId = updatedParent.ParentId;
                    updated = true;
                }
            }

            //  See if we need to update any children
            string[] childIDs = ChildLocations.Select(p => p.Id).ToArray();
            string[] locationsWithThisParent = locations.Locations.Where(p => string.Equals(Id, p.Value, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Key)
                .ToArray();
            string[] locationsWithoutThisParent = locations.Locations.Where(p => !string.Equals(Id, p.Value, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Key)
                .ToArray();

            string[] locationsToAdd = locationsWithThisParent.Where(p => !childIDs.Contains(p, StringComparer.OrdinalIgnoreCase)).ToArray();
            string[] locationsToRemove = locationsWithoutThisParent.Where(p => childIDs.Contains(p, StringComparer.OrdinalIgnoreCase)).ToArray();

            if (locationsToAdd.Any() || locationsToRemove.Any())
            {
                LocationListItem[] locationData = new LocationListItem[locationsToAdd.Length];
                for (int i = 0; i < locationsToAdd.Length; i++)
                {
                    string locationID = locationsToAdd[i];
                    Location location = await _service.GetLocation(locationID, cancellationToken);
                    locationData[i] = new LocationListItem() { Id = locationID, Name = location.Name, Tags = location.Tags };
                }

                _location.ChildLocations = ChildLocations.Where(p => !locationsToRemove.Contains(p.Id, StringComparer.OrdinalIgnoreCase))
                    .Concat(locationData)
                    .ToArray();

                updated = true;
            }
            return updated;
        }
    }
}
