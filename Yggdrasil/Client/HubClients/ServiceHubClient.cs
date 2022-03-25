using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Yggdrasil.Models;
using Yggdrasil.Models.Locations;
using Yggdrasil.SignalR;

namespace Yggdrasil.Client.HubClients
{
    /// <summary>
    /// Client for receiving SignalR messages at the service level
    /// </summary>
    public sealed class ServiceHubClient : ClientBase, IServiceHubClient
    {
        public ServiceHubClient(string uri, ILoggerProvider loggingProvider, Func<Task<string>> jwtProvider)
            : base(new Uri($"{uri}hub/service"), loggingProvider, jwtProvider)
        {
            AddMessageHandler<CampaignUpdatedEventArgs, CampaignOverview>(nameof(ServiceHubMethods.CampaignAdded), p => CampaignAdded?.Invoke(this, p));
            AddMessageHandler<CampaignUpdatedEventArgs, CampaignOverview>(nameof(ServiceHubMethods.CampaignUpdated), p => CampaignUpdated?.Invoke(this, p));
            AddMessageHandler<ItemRemovedEventArgs, string>(nameof(ServiceHubMethods.CampaignRemoved), p => CampaignRemoved?.Invoke(this, p));
            AddMessageHandler<PlayerCharacterUpdatedEventArgs, CampaignPlayerCharacter>(nameof(ServiceHubMethods.PCAdded), p => PlayerCharacterAdded?.Invoke(this, p));
            AddMessageHandler<PlayerCharacterUpdatedEventArgs, CampaignPlayerCharacter>(nameof(ServiceHubMethods.PCUpdated), p => PlayerCharacterUpdated?.Invoke(this, p));
            AddMessageHandler<ItemRemovedEventArgs, string>(nameof(ServiceHubMethods.PCRemoved), p => PlayerCharacterRemoved?.Invoke(this, p));
            AddMessageHandler<LocationEventArgs, Location>(nameof(ServiceHubMethods.LocationAdded), p => LocationAdded?.Invoke(this, p));
            AddMessageHandler<ItemRemovedEventArgs, string>(nameof(ServiceHubMethods.LocationRemoved), p => LocationRemoved?.Invoke(this, p));
            AddMessageHandler<LocationEventArgs, Location>(nameof(ServiceHubMethods.LocationUpdated), p => LocationUpdated?.Invoke(this, p));
            AddMessageHandler<LocationMovedEventArgs, LocationsMoved>(nameof(ServiceHubMethods.LocationsMoved), p => LocationsMoved?.Invoke(this, p));
        }

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
        /// <summary>
        /// Event that is triggered when a location is added
        /// </summary>
        public event EventHandler<LocationEventArgs> LocationAdded;
        /// <summary>
        /// Event that is triggered when a location is removed
        /// </summary>
        public event EventHandler<ItemRemovedEventArgs> LocationRemoved;
        /// <summary>
        /// Event that is triggered when a location is updated
        /// </summary>
        public event EventHandler<LocationEventArgs> LocationUpdated;
        /// <summary>
        /// Event that is triggered when a location is moved
        /// </summary>
        public event EventHandler<LocationMovedEventArgs> LocationsMoved;
    }
}
