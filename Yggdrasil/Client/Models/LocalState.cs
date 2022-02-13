using Yggdrasil.Client.HubClients;

namespace Yggdrasil.Client.Models
{
    public sealed class LocalState
    {
        /// <summary>
        /// Gets the hub used to get administrative events
        /// </summary>
        public AdminHubClient AdminHub { get; init; }
        /// <summary>
        /// Gets the hub used to get service level events
        /// </summary>
        public ServiceHubClient ServiceHub { get; init; }
    }
}
