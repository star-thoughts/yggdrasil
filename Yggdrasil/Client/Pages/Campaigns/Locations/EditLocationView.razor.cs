using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading.Tasks;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Pages.Components;
using Yggdrasil.Client.Pages.Exceptions;
using Yggdrasil.Client.Services;
using Yggdrasil.Client.ViewModels;
using Yggdrasil.Models.Locations;

namespace Yggdrasil.Client.Pages.Campaigns.Locations
{
    public partial class EditLocationView
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
        /// Gets or sets the ID of the location to edit
        /// </summary>
        [Parameter]
        public string ID { get; set; }
        /// <summary>
        /// Gets or sets the location to edit
        /// </summary>
        LocationViewModel Location { get; set; }
        /// <summary>
        /// Gets or sets whether this control is busy loading something
        /// </summary>
        public bool IsBusy { get; set; }

        ExceptionDialog ExceptionDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            IsBusy = true;
            await InvokeAsync(StateHasChanged);
            try
            {
                Location location = await CampaignService.GetLocation(ID);
                Location = new LocationViewModel(location, CampaignService);
            }
            finally
            {
                IsBusy = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        async Task SaveLocation()
        {
            IsBusy = true;
            await InvokeAsync(StateHasChanged);
            try
            {
                await Location.Save();
                NavigationManager.ViewLocation(Location.Id);
            }
            catch (Exception exc)
            {
                await ExceptionDialog.Show(exc);
            }
            finally
            {
                IsBusy = false;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
