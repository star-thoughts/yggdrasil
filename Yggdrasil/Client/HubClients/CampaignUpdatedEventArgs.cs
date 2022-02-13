using Yggdrasil.Models;

namespace Yggdrasil.Client.HubClients
{
    public class CampaignUpdatedEventArgs : BaseEventArgs
    {
        public CampaignUpdatedEventArgs(string editingUser, CampaignOverview campaign)
            : base(editingUser)
        {
            Campaign = campaign;
        }

        /// <summary>
        /// Gets the campaign that was updated
        /// </summary>
        public CampaignOverview Campaign { get; init; }
    }
}