﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Pages.Components;
using Yggdrasil.Client.Pages.Exceptions;
using Yggdrasil.Client.Services;
using Yggdrasil.Client.ViewModels;
using Yggdrasil.Models.Locations;

namespace Yggdrasil.Client.Pages.Campaigns.Locations
{
    /// <summary>
    /// View for managing a list of locations
    /// </summary>
    public partial class LocationListView : IDisposable
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
        /// Gets or sets the root location to display
        /// </summary>
        public string LocationID { get; set; }
        /// <summary>
        /// Gets or sets the Location to display
        /// </summary>
        LocationViewModel Location { get; set; }
        /// <summary>
        /// Gets the control that display a list of locations
        /// </summary>
        LocationsList LocationList { get; set; }
        IEnumerable<RootMapItem> Ancestors
        {
            get
            {
                return Location.ToRootMap();
            }
        }

        AddLocationDialog AddLocationDialog { get; set; }
        ExceptionDialog ExceptionDialog { get; set; }

        protected override void OnInitialized()
        {
            if (Globals != null)
            {
                Globals.ServiceHub.LocationRemoved += ServiceHub_LocationRemoved;
                Globals.ServiceHub.LocationsMoved += ServiceHub_LocationsMoved;
                Globals.ServiceHub.LocationUpdated += ServiceHub_LocationUpdated;
            }
        }

        private async void ServiceHub_LocationUpdated(object sender, HubClients.LocationEventArgs e)
        {
            if (Location.TryUpdate(e.Location))
                await InvokeAsync(StateHasChanged);
        }

        private async void ServiceHub_LocationsMoved(object sender, HubClients.LocationMovedEventArgs e)
        {
            if (await Location.TryUpdate(e.LocationsMoved))
                await InvokeAsync(StateHasChanged);
        }

        private async void ServiceHub_LocationRemoved(object sender, HubClients.ItemRemovedEventArgs e)
        {
            if (string.Equals(Location?.ID, e.ItemID, StringComparison.OrdinalIgnoreCase))
            {
                await MoveToAncestor(Location?.Ancestors?.FirstOrDefault()?.ID);
            }
        }

        /// <summary>
        /// Adds a new location as a child of the current LocationID, or to the root if LocationID is null or empty
        /// </summary>
        /// <returns>Task for asynchronous completion</returns>
        async Task AddLocation()
        {
            string id = await AddLocationDialog.Show(CampaignService, LocationID);
            if (!string.IsNullOrWhiteSpace(id))
            {
                NavigationManager.EditLocation(id);
            }
        }

        async Task MoveToAncestor(string itemID)
        {
            try
            {
                LocationID = itemID;
                if (!string.IsNullOrWhiteSpace(LocationID))
                    Location = await LocationViewModel.Create(itemID, CampaignService);
                else
                    Location = null;

                if (Location != null)
                    await LocationList.NavigateTo(Location);
                else
                    await LocationList.NavigateToRoot();

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception exc)
            {
                await ExceptionDialog.Show(exc);
            }
        }

        /// <summary>
        /// Disposes of this object
        /// </summary>
        public void Dispose()
        {
            if (Globals != null)
            {
                Globals.ServiceHub.LocationRemoved -= ServiceHub_LocationRemoved;
                Globals.ServiceHub.LocationsMoved -= ServiceHub_LocationsMoved;
                Globals.ServiceHub.LocationUpdated -= ServiceHub_LocationUpdated;
            }
        }

        void OpenLocation(string locationID)
        {
            string uri = NavigationManager.GetLocationAddress(locationID);
            NavigationManager.NavigateTo(uri);
        }
    }
}
