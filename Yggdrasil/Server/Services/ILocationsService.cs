using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Models.Locations;

namespace Yggdrasil.Server.Services
{
    /// <summary>
    /// Base interface for managing locations in the system
    /// </summary>
    public interface ILocationsService
    {
        /// <summary>
        /// Gets the root locations for the given campaign
        /// </summary>
        /// <param name="campaignId">ID of the campaign to get the root locations for</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Details of the root locations in the campaign</returns>
        Task<LocationsList> GetRootLocations(string campaignId, CancellationToken cancellationToken = default);
    }
}