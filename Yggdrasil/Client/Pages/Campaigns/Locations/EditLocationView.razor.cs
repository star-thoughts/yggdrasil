using Microsoft.AspNetCore.Components;
using Yggdrasil.Client.ViewModels;

namespace Yggdrasil.Client.Pages.Campaigns.Locations
{
    public partial class EditLocationView
    {
        /// <summary>
        /// Gets or sets the ID of the location to edit
        /// </summary>
        [Parameter]
        public string ID { get; set; }
        /// <summary>
        /// Gets or sets the location to edit
        /// </summary>
        LocationViewModel Location { get; set; }
    }
}
