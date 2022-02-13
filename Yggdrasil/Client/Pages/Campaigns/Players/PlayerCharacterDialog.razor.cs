using Fiction.Controls.Dialog;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Pages.Exceptions;
using Yggdrasil.Client.Services;
using Yggdrasil.Models;

namespace Yggdrasil.Client.Pages.Campaigns.Players
{
    public sealed partial class PlayerCharacterDialog : IDisposable
    {
        [Inject]
        ICampaignService CampaignService { get; set; }
        [CascadingParameter]
        LocalState LocalState { get; set; }
        /// <summary>
        /// Gets or sets the character name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets the player's account name
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// Gets or sets a collection of user names that can be assigned to players
        /// </summary>
        string[] UserNames { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Gets or sets the character that is being edited or added
        /// </summary>
        public CampaignPlayerCharacter _character;

        bool _isBusy;

        Dialog Dialog;
        ExceptionDialog ExceptionDialog;
        MessageBox MessageBox;

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Name);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        public Task Show(CampaignPlayerCharacter character)
        {
            _character = character;

            return Dialog.Show();
        }

        private async Task Connect()
        {
            try
            {
                UserNames = (await CampaignService.GetCampaignUsers())
                    .Select(p => p.UserName)
                    .OrderBy(p => p)
                    .ToArray();

                if (_character != null)
                {
                    Name = _character.Name;
                    UserName = _character.UserName;
                }

                LocalState.ServiceHub.PlayerCharacterRemoved += ServiceHub_PlayerCharacterRemoved;
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception exc)
            {
                await ExceptionDialog.Show(exc);
            }
        }

        private async void ServiceHub_PlayerCharacterRemoved(object sender, HubClients.ItemRemovedEventArgs e)
        {
            if (string.Equals(e.ItemID, _character?.ID, StringComparison.OrdinalIgnoreCase))
            {
                await MessageBox.ShowMessage("Character Removed", $"The character '{_character.Name} has been removed.");
                await Dialog.CloseDialog(false);
            }
        }

        async Task OnAccept()
        {
            try
            {
                CampaignPlayerCharacter character = new CampaignPlayerCharacter()
                {
                    ID = _character?.ID,
                    UserName = UserName,
                    Name = Name,
                };

                if (!string.IsNullOrWhiteSpace(character.ID))
                    await CampaignService.UpdatePlayerCharacter(character);
                else
                    await CampaignService.CreatePlayerCharacter(character);

                await Dialog.CloseDialog(true);
            }
            catch (Exception exc)
            {
                await ExceptionDialog.Show(exc);
            }
        }

        void OnCancel()
        {
            Dialog.CloseDialog(false);
        }

        async Task DialogOpened()
        {
            _isBusy = false;
            await InvokeAsync(StateHasChanged);
            try
            {
                await Connect();
            }
            finally
            {
                _isBusy = true;
                await InvokeAsync(StateHasChanged);
            }
        }

        void DialogClosing()
        {
            if (LocalState?.ServiceHub != null)
                LocalState.ServiceHub.PlayerCharacterRemoved -= ServiceHub_PlayerCharacterRemoved;
        }

        public void Dispose()
        {
            if (LocalState?.ServiceHub != null)
                LocalState.ServiceHub.PlayerCharacterRemoved -= ServiceHub_PlayerCharacterRemoved;
        }
    }
}
