using System;

namespace Yggdrasil.Client.HubClients
{
    /// <summary>
    /// Base interface for a service hub client for receiving messages from the server
    /// </summary>
    public interface IServiceHubClient
    {
        /// <summary>
        /// Event that is triggered when a campaign is added
        /// </summary>
        public event EventHandler<CampaignUpdatedEventArgs> CampaignAdded;
        /// <summary>
        /// Event that is triggered when a campaign is updated
        /// </summary>
        public event EventHandler<CampaignUpdatedEventArgs> CampaignUpdated;
        /// <summary>
        /// Event that is triggered when a campaign is removed
        /// </summary>
        public event EventHandler<ItemRemovedEventArgs> CampaignRemoved;
        /// <summary>
        /// Event that is triggered when a player character is added
        /// </summary>
        public event EventHandler<PlayerCharacterUpdatedEventArgs> PlayerCharacterAdded;
        /// <summary>
        /// Event that is triggered when a player character is updated
        /// </summary>
        public event EventHandler<PlayerCharacterUpdatedEventArgs> PlayerCharacterUpdated;
        /// <summary>
        /// Event that is triggered when a player character is removed
        /// </summary>
        public event EventHandler<ItemRemovedEventArgs> PlayerCharacterRemoved;
    }
}
