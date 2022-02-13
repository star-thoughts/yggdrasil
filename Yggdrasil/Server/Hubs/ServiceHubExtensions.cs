using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Yggdrasil.Models;
using Yggdrasil.SignalR;

namespace Yggdrasil.Server.Hubs
{
    /// <summary>
    /// Helper methods to invoke specific calls
    /// </summary>
    public static class ServiceHubExtensions
    {
        public static Task CampaignAdded(this IHubContext<ServiceHub> hub, string editingUser, CampaignOverview campaign)
        {
            if (hub == null)
                throw new ArgumentNullException(nameof(hub));

            return hub.Clients.All.SendAsync(ServiceHubMethods.CampaignAdded, editingUser, campaign);
        }

        public static Task CampaignRemoved(this IHubContext<ServiceHub> hub, string editingUser, string id)
        {
            if (hub == null)
                throw new ArgumentNullException(nameof(hub));

            return hub.Clients.All.SendAsync(ServiceHubMethods.CampaignRemoved, editingUser, id);
        }

        public static Task CampaignUpdated(this IHubContext<ServiceHub> hub, string editingUser, CampaignOverview campaign)
        {
            if (hub == null)
                throw new ArgumentNullException(nameof(hub));

            return hub.Clients.All.SendAsync(ServiceHubMethods.CampaignUpdated, editingUser, campaign);
        }

        public static Task PlayerCharacterAdded(this IHubContext<ServiceHub> hub, string campaignID, string editingUser, CampaignPlayerCharacter character)
        {
            if (hub == null)
                throw new ArgumentNullException(nameof(hub));

            return hub.Clients.Group(campaignID).SendAsync(ServiceHubMethods.PCAdded, editingUser, character);
        }

        public static Task PlayerCharacterUpdated(this IHubContext<ServiceHub> hub, string campaignID, string editingUser, CampaignPlayerCharacter character)
        {
            if (hub == null)
                throw new ArgumentNullException(nameof(hub));

            return hub.Clients.Group(campaignID).SendAsync(ServiceHubMethods.PCUpdated, editingUser, character);
        }

        public static Task PlayerCharacterRemoved(this IHubContext<ServiceHub> hub, string campaignID, string editingUser, string characterID)
        {
            if (hub == null)
                throw new ArgumentNullException(nameof(hub));

            return hub.Clients.Group(campaignID).SendAsync(ServiceHubMethods.PCRemoved, editingUser, characterID);
        }
    }
}
