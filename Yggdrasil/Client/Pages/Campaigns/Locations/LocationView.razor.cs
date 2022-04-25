using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Pages.Components;
using Yggdrasil.Client.Pages.Exceptions;
using Yggdrasil.Client.Services;
using Yggdrasil.Client.ViewModels;
using Yggdrasil.Models.Locations;

namespace Yggdrasil.Client.Pages.Campaigns.Locations
{
    /// <summary>
    /// View for display information about a location
    /// </summary>
    public partial class LocationView
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
        /// <summary>
        /// Gets or sets global objects for the page
        /// </summary>
        [CascadingParameter]
        LocalState Globals { get; set; }
        /// <summary>
        /// Gets or sets the ID this location is for
        /// </summary>
        [Parameter]
        public string LocationID { get; set; }
        /// <summary>
        /// Gets or sets whether the page is busy communicating with the server
        /// </summary>
        bool IsBusy { get; set; }
        /// <summary>
        /// Gets or sets the location to view
        /// </summary>
        public LocationViewModel Location { get; set; }
        /// <summary>
        /// Gets the current location's ancestors
        /// </summary>
        public IEnumerable<RootMapItem> Ancestors => Location?.Ancestors?.ToRootMap();

        ExceptionDialog ExceptionDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadLocation(LocationID);
            await base.OnInitializedAsync();
        }

        private async Task LoadLocation(string locationID)
        {
            if (IsValidLocationID(locationID))
            {
                IsBusy = true;
                await InvokeAsync(StateHasChanged);
                try
                {
                    Location location = await CampaignService.GetLocation(locationID);
                    Location = new LocationViewModel(location, CampaignService);
                    LocationID = locationID;
                }
                finally
                {
                    IsBusy = false;
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task NavigateToLocation(string locationID)
        {
            if (IsValidLocationID(locationID))
            {
                try
                {
                    await LoadLocation(locationID);
                    NavigationManager.NavigateTo($"/location/{HttpUtility.UrlEncode(locationID)}");
                }
                catch (Exception ex)
                {
                    await ExceptionDialog.Show(ex);
                }
            }
        }

        private bool IsValidLocationID(string locationID)
        {
            return !string.IsNullOrEmpty(locationID);
        }
    }
}
