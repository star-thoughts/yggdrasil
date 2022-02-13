using Fiction.Controls.Dialog;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Yggdrasil.Client.Pages.Exceptions;
using Yggdrasil.Client.Services;
using Yggdrasil.Models;

namespace Yggdrasil.Client.Pages.Campaigns
{
    public partial class CampaignDetailsDialog
    {
        [Inject]
        ICampaignService CampaignService { get; set; }
        bool IsValid
        {
            get { return !string.IsNullOrWhiteSpace(_campaignName); }
        }

        bool _busy = true;
        string _campaignName;
        string _campaignDescription;
        CampaignOverview _campaign;

        Dialog Dialog;
        ExceptionDialog ExceptionDialog;

        public async Task<string> Show(CampaignOverview campaign)
        {
            _campaign = campaign;

            _campaignName = _campaign?.Name;
            _campaignDescription = _campaign?.ShortDescription;
            _busy = false;

            return await Dialog.Show() as string;
        }

        async void OnAccept()
        {
            _busy = true;
            await InvokeAsync(StateHasChanged);
            try
            {
                string id = _campaign?.ID;

                if (string.IsNullOrWhiteSpace(id))
                    id = await CampaignService.CreateCampaign(_campaignName, _campaignDescription);
                else
                    await CampaignService.UpdateCampaign(id, _campaignName, _campaignDescription);

                await Dialog.CloseDialog(id);
            }
            catch (Exception exc)
            {
                await ExceptionDialog.Show(exc);
            }
            finally
            {
                _busy = false;
            await InvokeAsync(StateHasChanged);
            }
        }

        void OnCancel()
        {
            Dialog.CloseDialog(null);
        }
    }
}
