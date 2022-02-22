using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Yggdrasil.Client.Identity;
using Yggdrasil.Models;
using Yggdrasil.Models.Locations;

namespace Yggdrasil.Client.Services
{
    /// <summary>
    /// Service to communicate with the server for campaign information
    /// </summary>
    public sealed class CampaignService : ServiceBase, ICampaignService
    {
        /// <summary>
        /// Constructs a new <see cref="CampaignService"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="stateProvider">Login information state provider</param>
        public CampaignService(IHttpClientFactory factory, AuthenticationStateProvider stateProvider)
            : base(factory)
        {
            _stateProvider = stateProvider as ApiAuthenticationStateProvider;
        }

        private readonly ApiAuthenticationStateProvider _stateProvider;

        #region Campaigns
        /// <summary>
        /// Gets a list of campaigns from the server
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Campaign details in the system</returns>
        public async Task<CampaignList> GetCampaigns(CancellationToken cancellationToken = default)
        {
            string uri = "api/campaigns";

            using (HttpResponseMessage response = await GetClient().GetAsync(uri))
            {
                await CheckResponseForErrors(response);

                return await Deserialize<CampaignList>(response);
            }
        }

        /// <summary>
        /// Creates a new campaign and returns the campaign's ID
        /// </summary>
        /// <param name="name">Name of the campaign to create</param>
        /// <param name="shortDescription">A short description viewable by players for the campaign</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>The ID of the campaign that was created</returns>
        public async Task<string> CreateCampaign(string name, string shortDescription, CancellationToken cancellationToken = default)
        {
            string uri = "api/campaigns";

            uri = QueryHelpers.AddQueryString(uri, new Dictionary<string, string>()
            {
                { "name", name },
                { "description", shortDescription },
            });

            using (HttpContent content = new StringContent(string.Empty))
            {
                using (HttpResponseMessage response = await GetClient().PostAsync(uri, content, cancellationToken))
                {
                    await CheckResponseForErrors(response);

                    CreateCampaignResult result = await Deserialize<CreateCampaignResult>(response, cancellationToken);
                    return result.ID;
                }
            }
        }
        /// <summary>
        /// Deletes the campaign with the given ID
        /// </summary>
        /// <param name="campaignID">ID of the campaign to delete</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task DeleteCampaign(string campaignID, CancellationToken cancellationToken = default)
        {
            string uri = $"api/campaigns/{HttpUtility.UrlEncode(campaignID)}";

            using (HttpResponseMessage response = await GetClient().DeleteAsync(uri))
            {
                await CheckResponseForErrors(response);
            }
        }
        /// <summary>
        /// Deletes the current campaign
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task DeleteCampaign(CancellationToken cancellationToken = default)
        {
            string uri = $"api/campaigns";

            using (HttpResponseMessage response = await GetClient().DeleteAsync(uri))
            {
                await CheckResponseForErrors(response);
            }
        }

        /// <summary>
        /// Updates a campaign with new details
        /// </summary>
        /// <param name="campaignID">ID of the campaign to update</param>
        /// <param name="name">Name to give the campaign</param>
        /// <param name="shortDescription">A short description viewable by players for the campaign</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task UpdateCampaign(string campaignID, string name, string shortDescription, CancellationToken cancellationToken = default)
        {
            string uri = $"api/campaigns/{HttpUtility.UrlEncode(campaignID)}";

            uri = QueryHelpers.AddQueryString(uri, new Dictionary<string, string>()
            {
                { "name", name },
                { "description", shortDescription },
            });

            using (HttpContent content = new StringContent(string.Empty))
            {
                using (HttpResponseMessage response = await GetClient().PostAsync(uri, content, cancellationToken))
                {
                    await CheckResponseForErrors(response);
                }
            }
        }

        /// <summary>
        /// Opens a campaign, and gets the user's campaign permissions
        /// </summary>
        /// <param name="campaignID">ID of the campaign to get</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Campaign details in the system</returns>
        public async Task<OpenCampaignResult> OpenCampaign(string campaignID, CancellationToken cancellationToken = default)
        {
            string uri = $"api/campaigns/{HttpUtility.UrlEncode(campaignID)}/token";

            using (HttpResponseMessage response = await GetClient().GetAsync(uri))
            {
                await CheckResponseForErrors(response);

                OpenCampaignResult result = await Deserialize<OpenCampaignResult>(response);

                ApiAuthenticationStateProvider.JwtToken = result.CamapignToken;
                await _stateProvider.MarkUserAsAuthenticated(result.CamapignToken);

                return result;
            }
        }

        /// <summary>
        /// Gets information on the open campaign
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Campaign data</returns>
        public async Task<GetCampaignResult> GetCampaign(CancellationToken cancellationToken = default)
        {
            string uri = $"api/campaigns/overview";

            using (HttpResponseMessage response = await GetClient().GetAsync(uri))
            {
                await CheckResponseForErrors(response);

                GetCampaignResult result = await Deserialize<GetCampaignResult>(response);

                return result;
            }
        }
        #endregion Campaigns
        #region Campaigns
        /// <summary>
        /// Gets the characters of the currently open campaign
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Player character data</returns>
        public async Task<CampaignPlayerCharacters> GetCharacters(CancellationToken cancellationToken = default)
        {
            string uri = "api/campaigns/playercharacters";

            using (HttpResponseMessage response = await GetClient().GetAsync(uri, cancellationToken))
            {
                await CheckResponseForErrors(response);

                return await Deserialize<CampaignPlayerCharacters>(response, cancellationToken);
            }
        }

        /// <summary>
        /// Claims the character with the given ID for the current user
        /// </summary>
        /// <param name="characterID">ID of the charater to claim</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task ClaimCharacter(string characterID, CancellationToken cancellationToken = default)
        {
            string uri = $"api/campaigns/playercharaters/claim/{HttpUtility.UrlEncode(characterID)}";

            using (StringContent content = new StringContent(string.Empty))
            {
                using (HttpResponseMessage response = await GetClient().PostAsync(uri, content, cancellationToken))
                {
                    await CheckResponseForErrors(response);
                }
            }
        }

        /// <summary>
        /// Gets a list of users assigned to the campaign
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Collection of user data for users in the campaign</returns>
        public async Task<IEnumerable<CampaignUserData>> GetCampaignUsers(CancellationToken cancellationToken = default)
        {
            string uri = "api/campaigns/users";

            using (HttpResponseMessage response = await GetClient().GetAsync(uri, cancellationToken))
            {
                await CheckResponseForErrors(response);

                return await Deserialize<IEnumerable<CampaignUserData>>(response, cancellationToken);
            }
        }

        /// <summary>
        /// Updates player character data
        /// </summary>
        /// <param name="character">Player character to update</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task UpdatePlayerCharacter(CampaignPlayerCharacter character, CancellationToken cancellationToken = default)
        {
            string uri = "api/campaigns/playercharacters";

            using (HttpResponseMessage response = await GetClient().PutAsJsonAsync(uri, character, cancellationToken))
            {
                await CheckResponseForErrors(response);
            }
        }

        /// <summary>
        /// Creates a new player character
        /// </summary>
        /// <param name="character">Player character to create</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task CreatePlayerCharacter(CampaignPlayerCharacter character, CancellationToken cancellationToken = default)
        {
            string uri = "api/campaigns/playercharacters";

            using (HttpResponseMessage response = await GetClient().PostAsJsonAsync(uri, character, cancellationToken))
            {
                await CheckResponseForErrors(response);
            }
        }

        /// <summary>
        /// Removes a character from the campaign
        /// </summary>
        /// <param name="characterID">ID of the character to remove</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task RemoveCharacter(string characterID, CancellationToken cancellationToken = default)
        {
            string uri = $"api/campaigns/playercharacters/{characterID}";

            using (HttpResponseMessage response = await GetClient().DeleteAsync(uri, cancellationToken))
            {
                await CheckResponseForErrors(response);
            }
        }
        #endregion Characters
        #region Locations

        /// <summary>
        /// Gets a list of root locations for the campaign
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Collection of location information for listing</returns>
        public async Task<IEnumerable<LocationListItem>> GetRootLocations(CancellationToken cancellationToken = default)
        {
            string uri = "api/campaigns/locations";

            using (HttpResponseMessage response = await GetClient().GetAsync(uri, cancellationToken))
            {
                await CheckResponseForErrors(response);

                return (await Deserialize<LocationsList>(response)).Locations?.ToArray();
            }
        }

        /// <summary>
        /// Creates a location and returns the new location's ID
        /// </summary>
        /// <param name="name">Name to give the location</param>
        /// <param name="description">Optional description for the location</param>
        /// <param name="population">Optional population data for the location</param>
        /// <param name="tags">Optional tags for the location</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>ID of the new location</returns>
        public async Task<string> CreateLocation(string name, string description, Population population, string[] tags, CancellationToken cancellationToken = default)
        {
            string uri = "api/campaigns/locations";

            AddLocationData data = new AddLocationData()
            {
                Name = name,
                Description = description,
                Population = population,
                Tags = tags
            };

            using (HttpResponseMessage response = await GetClient().PostAsJsonAsync(uri, data, cancellationToken))
            {
                await CheckResponseForErrors(response);
                return await Deserialize<string>(response);
            }
        }

        /// <summary>
        /// Gets all of the data for a location
        /// </summary>
        /// <param name="locationID">ID of the location to get</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Location's information</returns>
        public async Task<Location> GetLocation(string locationID, CancellationToken cancellationToken = default)
        {
            string uri = $"api/campaigns/locations/{HttpUtility.UrlEncode(locationID)}";

            using (HttpResponseMessage response = await GetClient().GetAsync(uri, cancellationToken))
            {
                await CheckResponseForErrors(response);
                return await Deserialize<Location>(response);
            }
        }

        /// <summary>
        /// Deletes the given location from the campaign
        /// </summary>
        /// <param name="locationID">ID of the location to delete</param>
        /// <param name="childrenHandling">Whether or not to relocate the children to this location's parent.  Otherwise they are deleted.</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        public async Task RemoveLocation(string locationID, HandleChildren childrenHandling, CancellationToken cancellationToken = default)
        {
            string uri = $"api/campaigns/locations/{HttpUtility.UrlEncode(locationID)}";

            uri = QueryHelpers.AddQueryString(uri, "childrenHandling", childrenHandling.ToString());

            using (HttpResponseMessage response = await GetClient().DeleteAsync(uri, cancellationToken))
            {
                await CheckResponseForErrors(response);
            }
        }
        #endregion Locations
    }
}
