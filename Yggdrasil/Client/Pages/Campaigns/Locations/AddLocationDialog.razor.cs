using Fiction.Controls.Dialog;
using System;
using System.Threading.Tasks;
using Yggdrasil.Client.Services;
using Yggdrasil.Client.ViewModels;

namespace Yggdrasil.Client.Pages.Campaigns.Locations
{
    /// <summary>
    /// Dialog to get the minimal information required to create a location, and creates it if the user accepts the dialog
    /// </summary>
    public partial class AddLocationDialog
    {
        AddLocationViewModel ViewModel { get; set; }
        Dialog Dialog { get; set; }
        bool IsBusy { get; set; }

        /// <summary>
        /// Shows the dialog with the given parent ID
        /// </summary>
        /// <param name="service">Service used to communicate with the server</param>
        /// <param name="parentID">ID of the parent to create the new location in</param>
        /// <returns>ID of the newly created location</returns>
        public async Task<string> Show(ICampaignService service, string parentID)
        {
            ViewModel = new AddLocationViewModel(service, parentID);
            return (string)await Dialog.Show();
        }

        async Task OnAccept()
        {
            if (ViewModel.IsValid)
            {
                IsBusy = true;
                await InvokeAsync(StateHasChanged);
                try
                {
                    string id = await ViewModel.Save();
                    await Dialog.CloseDialog(id);
                }
                catch (Exception exc)
                {
                    await Dialog.CloseDialog(exc);
                }
                finally
                {
                    IsBusy = false;
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        async Task OnCancel()
        {
            await Dialog.CloseDialog(string.Empty);
        }
    }
}
