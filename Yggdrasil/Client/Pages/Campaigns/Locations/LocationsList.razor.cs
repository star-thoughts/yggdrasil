using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Services;
using Yggdrasil.Client.ViewModels;
using Yggdrasil.Models.Locations;
using Yggdrasil.Client.Pages.Components;

namespace Yggdrasil.Client.Pages.Campaigns.Locations
{
    /// <summary>
    /// Component for showing a list of locations
    /// </summary>
    public partial class LocationsList
    {
        /// <summary>
        /// Gets the service used to manage campaign information
        /// </summary>
        [Inject]
        ICampaignService CampaignService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        AuthenticationStateProvider AuthProvider { get; set; }
        [Inject]
        IJSRuntime JSRuntime { get; set; }
        /// <summary>
        /// Gets or sets global objects for the page
        /// </summary>
        [CascadingParameter]
        LocalState Globals { get; set; }
        /// <summary>
        /// Gets or sets whether or not to automatically load the locations when the control first loads
        /// </summary>
        [Parameter]
        public bool AutoLoad { get; set; } = true;
        /// <summary>
        /// Gets or sets whether the user can change locations by selecting a location
        /// </summary>
        [Parameter]
        public bool CanChangeLocations { get; set; } = true;
        /// <summary>
        /// Gets or sets the parent location to display children for, or null to display root locations
        /// </summary>
        /// <remarks>
        /// <para>If <see cref="Locations"/> is explicitly set, then this parameter is ignored and the locations given are shown instead.</para>
        /// <para>Regardless of the value of <see cref="Locations"/>, if this parameter is set, then it responds to events about the Location.  For example,
        /// if Locations is set to null, and LocationID has a value, then all children added to, updated or removed from the location will show up in this list.  If
        /// LocationID is empty (not-null), then root location updates will be shown here.
        /// </remarks>
        [Parameter]
        public string LocationID { get; set; }
        /// <summary>
        /// Event that is triggered when the <see cref="LocationID"/> changes
        /// </summary>
        [Parameter]
        public EventCallback<string> LocationIDChanged { get; set; }
        /// <summary>
        /// Gets or sets the location to display children of
        /// </summary>
        /// <para>If <see cref="Locations"/> is explicitly set, then this parameter is ignored and the locations given are shown instead.</para>
        /// <para>Regardless of the value of <see cref="Locations"/>, if this parameter is set, then it responds to events about the Location.  For example,
        /// if Locations is set to null, and LocationID has a value, then all children added to, updated or removed from the location will show up in this list.  If
        /// LocationID is empty (not-null), then root location updates will be shown here.
        /// </remarks>
        [Parameter]
        public LocationViewModel Location { get; set; }
        /// <summary>
        /// Event that is triggered when the current <see cref="Location"/> changes
        /// </summary>
        [Parameter]
        public EventCallback<LocationViewModel> LocationChanged { get; set; }
        /// <summary>
        /// Gets or sets the locations to display.  If not set, then it is auto-populated with children of <see cref="LocationID"/>
        /// </summary>
        [Parameter]
        public IEnumerable<LocationListItem> Locations { get; set; }
        /// <summary>
        /// Gets or sets the selected location
        /// </summary>
        [Parameter]
        public LocationListItem SelectedLocation { get; set; }
        /// <summary>
        /// Event that is triggered when the selected location changes
        /// </summary>
        [Parameter]
        public EventCallback<LocationListItem> SelectedLocationChanged { get; set; }
        /// <summary>
        /// Gets or sets whether or not this control is busy loading information
        /// </summary>
        private bool IsBusy { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadLocations();
        }

        /// <summary>
        /// Fills the <see cref="Locations"/> collection if the LocationID is specified
        /// </summary>
        /// <returns></returns>
        private async Task LoadLocations()
        {
            //  If no locations were provided, then use the location ID to find some
            IsBusy = true;
            await InvokeAsync(StateHasChanged);
            try
            {
                if (IsLocationSpecified())
                {
                    await UpdateFromLocation();
                }
                else if (IsLocationIDSpecified())
                {
                    await UpdateFromLocationID();
                }
                else
                    await UpdateFromRootLocations();
            }
            finally
            {
                IsBusy = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Forces this control to load location
        /// </summary>
        /// <param name="location">Location to navigate to</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task NavigateTo(LocationViewModel location)
        {
            LocationID = location?.Id;
            Location = location;
            Locations = null;
            await LoadLocations();
        }

        /// <summary>
        /// Forces this control to load the root location
        /// </summary>
        /// <param name="location">Location to navigate to</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task NavigateToRoot()
        {
            LocationID = null;
            Location = null;
            Locations = null;
            await LoadLocations();
        }

        private async Task UpdateFromRootLocations()
        {
            Locations = await CampaignService.GetRootLocations();
        }

        private async Task UpdateFromLocationID()
        {
            Location = await LocationViewModel.Create(LocationID, CampaignService);
            await LocationChanged.InvokeAsync(Location);
            Locations = Location?.ChildLocations;
        }

        private async Task UpdateFromLocation()
        {
            LocationID = Location.ID;
            await LocationIDChanged.InvokeAsync(LocationID);
            Locations = Location?.ChildLocations;
        }

        private bool IsLocationIDSpecified()
        {
            return !string.IsNullOrEmpty(LocationID);
        }

        private bool IsLocationSpecified()
        {
            return Location != null;
        }

        async Task NavigateToLocation(string locationID)
        {
            if (IsDifferentLocationID(locationID))
            {
                Locations = null;
                Location = null;
                LocationID = locationID;
                await LocationIDChanged.InvokeAsync(locationID);
                await LocationChanged.InvokeAsync(null);
                await LoadLocations();
            }
        }

        private bool IsDifferentLocationID(string locationID)
        {
            return !string.Equals(LocationID, locationID, System.StringComparison.OrdinalIgnoreCase);
        }

        async Task ItemDoubleClicked(LocationListItem location)
        {
            if (CanChangeLocations)
                await NavigateToLocation(location.Id);
        }

        async Task OnSelectedLocationChanged(LocationListItem location)
        {
            await SelectedLocationChanged.InvokeAsync(location);
        }

        async Task OpenLocation(string locationID, bool inNewWindow)
        {
            string uri = NavigationManager.GetLocationAddress(locationID);
            if (inNewWindow)
                await JSRuntime.InvokeAsync<object>("open", new object[] { uri, "_blank" });
            else
                NavigationManager.NavigateTo(uri);
        }
    }
}
