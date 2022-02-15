using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yggdrasil.Models;
using Yggdrasil.Models.Locations;
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

        public static Task PlayerCharacterAdded(this IHubContext<ServiceHub> hub, string campaignId, string editingUser, CampaignPlayerCharacter character)
        {
            if (hub == null)
                throw new ArgumentNullException(nameof(hub));

            return hub.Clients.Group(campaignId).SendAsync(ServiceHubMethods.PCAdded, editingUser, character);
        }

        public static Task PlayerCharacterUpdated(this IHubContext<ServiceHub> hub, string campaignId, string editingUser, CampaignPlayerCharacter character)
        {
            if (hub == null)
                throw new ArgumentNullException(nameof(hub));

            return hub.Clients.Group(campaignId).SendAsync(ServiceHubMethods.PCUpdated, editingUser, character);
        }

        public static Task PlayerCharacterRemoved(this IHubContext<ServiceHub> hub, string campaignId, string editingUser, string characterId)
        {
            if (hub == null)
                throw new ArgumentNullException(nameof(hub));

            return hub.Clients.Group(campaignId).SendAsync(ServiceHubMethods.PCRemoved, editingUser, characterId);
        }

        public static Task LocationAdded(this IHubContext<ServiceHub> hub, string campaignId, string editingUser, Location location)
        {
            ArgumentNullException.ThrowIfNull(hub);

            return hub.Clients.Group(campaignId).SendAsync(ServiceHubMethods.LocationAdded, editingUser, location);
        }

        public static Task LocationUpdated(this IHubContext<ServiceHub> hub, string campaignId, string editingUser, Location location)
        {
            ArgumentNullException.ThrowIfNull(hub);

            return hub.Clients.Group(campaignId).SendAsync(ServiceHubMethods.LocationUpdated, editingUser, location);
        }

        public static Task LocationRemoved(this IHubContext<ServiceHub> hub, string campaignId, string editingUser, string locationId)
        {
            ArgumentNullException.ThrowIfNull(hub);

            return hub.Clients.Group(campaignId).SendAsync(ServiceHubMethods.LocationRemoved, editingUser, locationId);
        }

        public static Task LocationsMoved(this IHubContext<ServiceHub> hub, string campaignId, string editingUser, LocationsMoved moved)
        {
            ArgumentNullException.ThrowIfNull(hub);

            return hub.Clients.Group(campaignId).SendAsync(ServiceHubMethods.LocationsMoved, editingUser, moved);
        }
    }
}
