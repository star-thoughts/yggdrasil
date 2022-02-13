using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Yggdrasil.Identity;

namespace Yggdrasil.Client.HubClients
{
    public class AdminHubClient : ClientBase
    {
        public AdminHubClient(string baseUri, ILoggerProvider loggingProvider, Func<Task<string>> jwtProvider)
            : base(new Uri($"{baseUri}hub/admin"), loggingProvider, jwtProvider)
        {
            AddMessageHandler<UserUpdatedEventArgs, UserInfo>(SignalR.AdminHubMethods.UserAdded, p => UserAdded?.Invoke(this, p));
            AddMessageHandler<UserUpdatedEventArgs, UserInfo>(SignalR.AdminHubMethods.UserUpdated, p => UserUpdated?.Invoke(this, p));
            AddMessageHandler<UserRemovedEventArgs, string>(SignalR.AdminHubMethods.UserRemoved, p => UserRemoved?.Invoke(this, p));
        }

        /// <summary>
        /// Event that is triggered when a user is added
        /// </summary>
        public event EventHandler<UserUpdatedEventArgs> UserAdded;
        /// <summary>
        /// Event that is triggered when a user is updated
        /// </summary>
        public event EventHandler<UserUpdatedEventArgs> UserUpdated;
        /// <summary>
        /// Event that is triggered when a user is removed
        /// </summary>
        public event EventHandler<UserRemovedEventArgs> UserRemoved;
    }
}
