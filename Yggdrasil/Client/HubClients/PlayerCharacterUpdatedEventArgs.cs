using Yggdrasil.Models;

namespace Yggdrasil.Client.HubClients
{
    public class PlayerCharacterUpdatedEventArgs : BaseEventArgs
    {
        public PlayerCharacterUpdatedEventArgs(string editingUser, CampaignPlayerCharacter character)
            : base(editingUser)
        {
            Character = character;
        }

        public CampaignPlayerCharacter Character { get; set; }
    }
}