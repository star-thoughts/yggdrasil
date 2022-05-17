using Microsoft.AspNetCore.Components;
using Radzen;
using System.Threading.Tasks;

namespace Yggdrasil.Client.Pages.Components
{
    public partial class MessageBoxDialog
    {
        [Inject]
        DialogService DialogService { get; set; }
        /// <summary>
        /// Gets or sets the message to display
        /// </summary>
        [Parameter]
        [SupplyParameterFromQuery]
        public MessageBoxOptions MessageBoxOptions { get; set; }

        public void ButtonClicked(MessageBoxResult button)
        {
            DialogService.Close(button);
        }
    }
}
