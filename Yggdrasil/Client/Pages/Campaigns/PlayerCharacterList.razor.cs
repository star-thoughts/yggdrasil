using Fiction.Controls.Dialog;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Pages.Campaigns.Players;
using Yggdrasil.Client.Pages.Exceptions;
using Yggdrasil.Client.Services;
using Yggdrasil.Models;

namespace Yggdrasil.Client.Pages.Campaigns
{
    public sealed partial class PlayerCharacterList : IDisposable
    {
        [Inject]
        ICampaignService CampaignService { get; set; }
        [CascadingParameter]
        LocalState LocalState { get; set; }
        /// <summary>
        /// Gets or sets the currently selected character
        /// </summary>
        [Parameter]
        public CampaignPlayerCharacter Selected { get; set; }
        [Parameter]
        public EventCallback<CampaignPlayerCharacter> SelectedChanged { get; set; }
        List<CampaignPlayerCharacter> Characters { get; set; }

        bool _loading;

        PlayerCharacterDialog PlayerCharacterDialog;
        MessageBox MessageBox;
        ExceptionDialog ExceptionDialog;

        protected override async Task OnInitializedAsync()
        {
            await Connect();

            await base.OnInitializedAsync();
        }

        async Task Connect()
        {
            try
            {
                _loading = true;

                Characters = GetSortedCharacters((await CampaignService.GetCharacters()).Characters);

                LocalState.ServiceHub.PlayerCharacterAdded += ServiceHub_PlayerCharacterUpdated;
                LocalState.ServiceHub.PlayerCharacterUpdated += ServiceHub_PlayerCharacterUpdated;
                LocalState.ServiceHub.PlayerCharacterRemoved += ServiceHub_PlayerCharacterRemoved;
            }
            catch
            {
                //  Swallow the exception, as assumably the parent controls would have seen them already
            }
            finally
            {
                _loading = false;
            }

            await InvokeAsync(StateHasChanged);
        }

        private async void ServiceHub_PlayerCharacterRemoved(object sender, HubClients.ItemRemovedEventArgs e)
        {
            CampaignPlayerCharacter character = Characters.FirstOrDefault(p => string.Equals(p.ID, e.ItemID, StringComparison.OrdinalIgnoreCase));

            Characters.Remove(character);

            await InvokeAsync(StateHasChanged);
        }

        private async void ServiceHub_PlayerCharacterUpdated(object sender, HubClients.PlayerCharacterUpdatedEventArgs e)
        {
            CampaignPlayerCharacter character = Characters.FirstOrDefault(p => string.Equals(p.ID, e.Character.ID, StringComparison.OrdinalIgnoreCase));
            Characters.Remove(character);

            Characters.Add(e.Character);
            Characters = GetSortedCharacters(Characters);

            if (ReferenceEquals(Selected, character))
            {
                Selected = e.Character;
                await SelectedChanged.InvokeAsync(e.Character);
            }

            await InvokeAsync(StateHasChanged);
        }

        List<CampaignPlayerCharacter> GetSortedCharacters(IEnumerable<CampaignPlayerCharacter> characters)
        {
            return characters
                .OrderBy(p => p.Name)
                .ThenBy(p => p.UserName)
                .ToList();
        }

        async void SelectedItemChanged(CampaignPlayerCharacter character)
        {
            Selected = character;
            await SelectedChanged.InvokeAsync(character);
            await InvokeAsync(StateHasChanged);
        }

        async Task CreateCharacter()
        {
            await PlayerCharacterDialog.Show(null);
        }

        async Task EditCharacter(CampaignPlayerCharacter character)
        {
            await PlayerCharacterDialog.Show(character);
        }

        async Task RemoveCharacter(CampaignPlayerCharacter character)
        {
            try
            {
                if (await MessageBox.Confirm("Delete Character", $"Delete {character.Name} from the campaign?"))
                    await CampaignService.RemoveCharacter(character.ID);
            }
            catch (Exception exc)
            {
              await  ExceptionDialog.Show(exc);
            }
        }

        /// <summary>
        /// Claims a player character for a user
        /// </summary>
        /// <param name="character">Player character to claim</param>
        async Task Claim(CampaignPlayerCharacter character)
        {
            bool result = await MessageBox.Confirm("Claim Character", $"Do you wish to claim '{character.Name}' as your character?");
            if (result)
            {
                try
                {
                    await CampaignService.ClaimCharacter(character.ID);
                }
                catch (Exception exc)
                {
                    await ExceptionDialog.Show(exc);
                }
            }
        }

        public void Dispose()
        {
            LocalState.ServiceHub.PlayerCharacterAdded -= ServiceHub_PlayerCharacterUpdated;
            LocalState.ServiceHub.PlayerCharacterUpdated -= ServiceHub_PlayerCharacterUpdated;
            LocalState.ServiceHub.PlayerCharacterRemoved -= ServiceHub_PlayerCharacterRemoved;
        }
    }
}
