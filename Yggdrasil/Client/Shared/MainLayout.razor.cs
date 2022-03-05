using Fiction.Controls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Pages.Auth;
using Yggdrasil.Identity;

namespace Yggdrasil.Client.Shared
{
    public sealed partial class MainLayout
    {
        /// <summary>
        /// Gets or sets the state povider used by the application
        /// </summary>
        [Inject]
        AuthenticationStateProvider StateProvider { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        ILoggerProvider LoggerProvider { get; set; }
        [Inject]
        AuthenticationStateProvider AuthProvider { get; set; }

        /// <summary>
        /// Gets the globals for use within sub-pages
        /// </summary>
        LocalState Globals { get; set; }

        RegisterDialog RegisterDialog;
        LoginDialog LoginDialog;

        NavBarEntry[] _navBarEntries = Array.Empty<NavBarEntry>();

        protected override async Task OnInitializedAsync()
        {
            Globals = new LocalState()
            {
                AdminHub = new HubClients.AdminHubClient(NavigationManager.BaseUri, LoggerProvider, () => Task.FromResult(Identity.ApiAuthenticationStateProvider.JwtToken)),
                ServiceHub = new HubClients.ServiceHubClient(NavigationManager.BaseUri, LoggerProvider, () => Task.FromResult(Identity.ApiAuthenticationStateProvider.JwtToken)),
            };

            AuthProvider.AuthenticationStateChanged += AuthProvider_AuthenticationStateChanged;

            await ConnectToSignalRClients();
            StateProvider.AuthenticationStateChanged += StateProvider_AuthenticationStateChangedAsync;
            await base.OnInitializedAsync();
        }

        private async void AuthProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            ClaimsPrincipal user = (await AuthProvider.GetAuthenticationStateAsync()).User;
            bool authenticated = user.Identity.IsAuthenticated;
            bool campaign = user.Claims.Any(p => string.Equals(p.Type, "campaign", StringComparison.OrdinalIgnoreCase));

            List<NavBarEntry> entries = new List<NavBarEntry>();

            if (authenticated)
            {
                if (campaign)
                {
                    var campaignEntry = new NavBarEntry() { Icon = "travel_explore", Label = "Campaign", Address = "/campaign" };
                    entries.Add(campaignEntry);
                    campaignEntry.Children = new NavBarEntry[]
                    {
                        new NavBarEntry() { Label = "Locations", Address = "/locations" },
                    };
                }
                if (user.IsInRole(Roles.ManageUsers))
                    entries.Add(new NavBarEntry() { Icon = "supervisor_account", Label = "Users", Address = "/users/manage" });
            }

            _navBarEntries = entries.ToArray();
        }

        private async void StateProvider_AuthenticationStateChangedAsync(Task<AuthenticationState> task)
        {
            await ReconnectToSignalRClients();
        }

        private async Task ReconnectToSignalRClients()
        {
            await Globals.AdminHub.Restart();
            await Globals.ServiceHub.Restart();
        }

        private async Task ConnectToSignalRClients()
        {
            await Globals.AdminHub.StopClient();
            await Globals.AdminHub.StartClient();
            await Globals.ServiceHub.StopClient();
            await Globals.ServiceHub.StartClient();
        }

        async Task OnLogin()
        {
            await LoginDialog.Show();
        }

        async Task OnRegister()
        {
            await RegisterDialog.Show();
        }

        IEnumerable<NavBarEntry> GetNavBarEntries()
        {
            return _navBarEntries;
        }
    }
}
