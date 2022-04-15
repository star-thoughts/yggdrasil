using Microsoft.AspNetCore.Components;

namespace Yggdrasil.Client.Pages.Campaigns.Locations
{
    public partial class EditLocationView
    {
        /// <summary>
        /// Gets or sets the ID of the location to edit
        /// </summary>
        [Parameter]
        public string ID { get; set; }
    }
}
