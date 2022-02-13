using Fiction.Controls.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Pages.Exceptions;
using Yggdrasil.Client.Services;
using Yggdrasil.Identity;
using Yggdrasil.Models;

namespace Yggdrasil.Client.Pages.Campaigns
{
    public sealed partial class CampaignView : IDisposable
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
        GetCampaignResult Campaign { get; set; }
        string DungeonMaster { get; set; } = string.Empty;
        string RoleBasedLayout { get; set; } = string.Empty;

        bool _loading = true;
        bool _authenticating;

        MessageBox MessageBox;
        ExceptionDialog ExceptionDialog;

        protected override async Task OnInitializedAsync()
        {
            AuthProvider.AuthenticationStateChanged += AuthProvider_AuthenticationStateChanged;
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (!(await AuthProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated)
                    await ShowNotAuthorized();
                else
                    await Connect();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async void AuthProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            if (!(await AuthProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated)
                await ShowNotAuthorized();

            if (!_authenticating)
                await Connect();
        }

        private async Task ShowNotAuthorized()
        {
            await MessageBox.ShowMessage("Unauthorized", "You must log in to view a campaign.");

            NavigationManager.NavigateTo("/");
        }

        private async Task Connect()
        {
            try
            {
                _authenticating = true;
                _loading = true;
                Campaign = await CampaignService.GetCampaign();
                DungeonMaster = Campaign.Users.FirstOrDefault(p => string.Equals(p.Roles, Roles.DungeonMaster, StringComparison.Ordinal))?.UserName;

                // Must get the authentication state after the call to OpenCampaign, since it changes the identity of the user
                if ((await AuthProvider.GetAuthenticationStateAsync()).User.IsInRole(Roles.DungeonMaster))
                    RoleBasedLayout = "layout-gm";
                else
                    RoleBasedLayout = "layout-player";

            }
            catch (Exception exc)
            {
                await ExceptionDialog.Show(exc);
                NavigationManager.NavigateTo("/");
            }
            finally
            {
                _authenticating = false;
                _loading = false;
            }

            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            if (AuthProvider != null)
                AuthProvider.AuthenticationStateChanged -= AuthProvider_AuthenticationStateChanged;
        }
    }
}
