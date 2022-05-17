using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Services;

namespace Yggdrasil.Client.Pages.Campaigns
{
    public partial class CampaignView
    {
        [Inject]
        DialogService DialogService { get; set; }
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
    }
}
