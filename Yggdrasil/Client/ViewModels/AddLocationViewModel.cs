using System;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Client.Services;

namespace Yggdrasil.Client.ViewModels
{
    /// <summary>
    /// View Model for adding a location
    /// </summary>
    public class AddLocationViewModel
    {
        /// <summary>
        /// Constructs a new <see cref="AddLocationViewModel"/>
        /// </summary>
        /// <param name="service">Service for communicating with the server</param>
        public AddLocationViewModel(ICampaignService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Constructs a new <see cref="AddLocationViewModel"/>
        /// </summary>
        /// <param name="service">Service for communicating with the server</param>
        public AddLocationViewModel(ICampaignService service, string parentID)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            ParentID = parentID;
        }

        private ICampaignService _service;

        /// <summary>
        /// Gets or sets the name of the location to add
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets the maximum length for a name
        /// </summary>
        public const int MaxNameLength = 50;
        /// <summary>
        /// Gets or sets the ID of the parent location for this new location
        /// </summary>
        public string ParentID { get; set; }
        /// <summary>
        /// Gets or sets whether this view model is valid
        /// </summary>
        public bool IsValid
        {
            get => !string.IsNullOrWhiteSpace(Name)
                    && Name.Length <= MaxNameLength;
        }

        /// <summary>
        /// Saves the location to the server, and returns the ID of the created location
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>ID of the created location</returns>
        public async Task<string> Save(CancellationToken cancellationToken = default)
        {
            if (IsValid)
                return await _service.CreateLocation(Name, ParentID, string.Empty, null, Array.Empty<string>(), cancellationToken);
            return string.Empty;
        }
    }
}
