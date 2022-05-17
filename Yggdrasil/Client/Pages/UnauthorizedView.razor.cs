using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using System;
using System.Threading.Tasks;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Pages.Components;
using Yggdrasil.Client.Services;

namespace Yggdrasil.Client.Pages
{
    /// <summary>
    /// View that is displayed when the user is not authorized to view a page or component
    /// </summary>
    public sealed partial class UnauthorizedView
    {
        /// <summary>
        /// Gets the service used to manage campaign information
        /// </summary>
        [Inject]
        IAuthService AuthService { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        AuthenticationStateProvider AuthProvider { get; set; }
        [Inject]
        DialogService DialogService { get; set; }
        /// <summary>
        /// Gets or sets global objects for the page
        /// </summary>
        [CascadingParameter]
        LocalState Globals { get; set; }

        async Task OnLogin(LoginArgs args)
        {
            await using (await Globals.GetBusyView().BeginOperation())
            {
                try
                {
                    await AuthService.Login(args.Username, args.Password);
                }
                catch (Exception exc)
                {
                    await DialogService.MessageBoxAsync("Error Logging In", exc.Message, MessageBoxType.Close);
                }
            }
        }

        void OnRegister()
        {
        }
    }
}
