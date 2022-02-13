using Fiction.Controls.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Pages.Campaigns;
using Yggdrasil.Client.Pages.Exceptions;
using Yggdrasil.Client.Services;
using Yggdrasil.Models;

namespace Yggdrasil.Client.Pages
{
    /// <summary>
    /// Main page that allows the user to pick campaigns or manage them
    /// </summary>
    public sealed partial class Index : IDisposable
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
        [CascadingParameter]
        LocalState Globals { get; set; }

        /// <summary>
        /// Gets or sets a collction of available campaigns
        /// </summary>
        List<CampaignOverview> Campaigns { get; set; }
        CampaignOverview SelectedCampaign { get; set; }
        bool _loading = true;

        ExceptionDialog ExceptionDialog;
        CampaignDetailsDialog CampaignDetailsDialog;
        MessageBox MessageBox;

        protected override async Task OnInitializedAsync()
        {
            AuthProvider.AuthenticationStateChanged += AuthProvider_AuthenticationStateChanged;
            await Connect((await AuthProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated);
            await base.OnInitializedAsync();
        }

        private async void AuthProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            await Connect((await task).User.Identity.IsAuthenticated);
        }

        private async Task Connect(bool authenticated)
        {
            DisconnectHub();

            Campaigns = new List<CampaignOverview>();
            if (authenticated)
            {
                try
                {
                    _loading = true;
                    await InvokeAsync(StateHasChanged);

                    Campaigns = GetOrderedCampaigns((await CampaignService.GetCampaigns()).Campaigns);
                    Globals.ServiceHub.CampaignUpdated += ServiceHub_CampaignUpdated;
                    Globals.ServiceHub.CampaignAdded += ServiceHub_CampaignUpdated;
                    Globals.ServiceHub.CampaignRemoved += ServiceHub_CampaignRemoved;
                }
                catch (Exception exc)
                {
                    await ExceptionDialog.Show(exc);
                }
                finally
                {
                    _loading = false;
                }
            }

            await InvokeAsync(StateHasChanged);
        }

        static List<CampaignOverview> GetOrderedCampaigns(IEnumerable<CampaignOverview> campaigns)
        {
            return campaigns.OrderBy(p => p.Name)
                .ToList();
        }

        private void ServiceHub_CampaignRemoved(object sender, HubClients.ItemRemovedEventArgs e)
        {
            CampaignOverview campaign = Campaigns.FirstOrDefault(p => string.Equals(p.ID, e.ItemID, StringComparison.OrdinalIgnoreCase));
            Campaigns.Remove(campaign);

            _ = InvokeAsync(StateHasChanged);
        }

        private void ServiceHub_CampaignUpdated(object sender, HubClients.CampaignUpdatedEventArgs e)
        {
            CampaignOverview current = Campaigns.FirstOrDefault(p => string.Equals(p.ID, e.Campaign.ID, StringComparison.OrdinalIgnoreCase));
            Campaigns.Remove(current);
            Campaigns.Add(current);
            Campaigns = GetOrderedCampaigns(Campaigns);

            _ = InvokeAsync(StateHasChanged);
        }

        void ViewCampaign(CampaignOverview campaign)
        {
            string id = campaign?.ID;
            if (!string.IsNullOrWhiteSpace(id))
                OpenCampaign(id);
        }

        async void OpenCampaign(string campaignID)
        {
            try
            {
                await CampaignService.OpenCampaign(campaignID);
                NavigationManager.NavigateTo($"campaign");
            }
            catch (Exception exc)
            {
                await ExceptionDialog.Show(exc);
            }
        }

        async void CreateCampaign()
        {
            string id = await CampaignDetailsDialog.Show(null);
            if (!string.IsNullOrWhiteSpace(id))
                OpenCampaign(id);
        }

        async void DeleteCampaign(CampaignOverview campaign)
        {
            if (await MessageBox.Confirm("Delete Campaign", $"Delete \"{campaign.Name}\"?"))
            {
                try
                {
                    await CampaignService.DeleteCampaign(campaign.ID);
                }
                catch (Exception exc)
                {
                    await ExceptionDialog.Show(exc);
                }
            }
        }

        public void Dispose()
        {
            DisconnectHub();
            AuthProvider.AuthenticationStateChanged -= AuthProvider_AuthenticationStateChanged;
        }

        private void DisconnectHub()
        {
            Globals.ServiceHub.CampaignUpdated -= ServiceHub_CampaignUpdated;
            Globals.ServiceHub.CampaignAdded -= ServiceHub_CampaignUpdated;
            Globals.ServiceHub.CampaignRemoved -= ServiceHub_CampaignRemoved;
        }
    }
}
