using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Yggdrasil.Client.Identity;
using Yggdrasil.Identity;

namespace Yggdrasil.Client.Services
{
    /// <summary>
    /// Service for authorizing and managing authorizations
    /// </summary>
    public sealed class AuthService : ServiceBase, IAuthService
    {
        /// <summary>
        /// Constructs a new <see cref="AuthService"/>
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="stateProvider">Login information state provider</param>
        public AuthService(IHttpClientFactory factory, AuthenticationStateProvider stateProvider)
            : base(factory)
        {
            _stateProvider = stateProvider as ApiAuthenticationStateProvider;
        }

        private readonly ApiAuthenticationStateProvider _stateProvider;

        public async Task<RegisterResult> Register(string userName, string password, CancellationToken cancellationToken = default)
        {
            RegisterRequest request = new RegisterRequest() { UserName = userName, Password = password };
            string uri = "api/auth/register";

            using (HttpResponseMessage response = await GetClient().PostAsJsonAsync(uri, request, cancellationToken))
            {
                await CheckResponseForErrors(response);

                return await Deserialize<RegisterResult>(response, cancellationToken);
            }
        }

        public async Task<LoginResponse> Login(string userName, string password, CancellationToken cancellationToken = default)
        {
            LoginRequest request = new LoginRequest() { UserName = userName, Password = password };
            string uri = "api/auth/login";

            using (HttpResponseMessage response = await GetClient().PostAsJsonAsync(uri, request, cancellationToken))
            {
                await CheckResponseForErrors(response);

                LoginResponse result = await Deserialize<LoginResponse>(response, cancellationToken);
                ApiAuthenticationStateProvider.JwtToken = result.Token;
                await _stateProvider.MarkUserAsAuthenticated(result.Token);
                return result;
            }
        }

        public async Task DeleteUser(string userName, CancellationToken cancellationToken = default)
        {
            string uri = $"api/auth/users/{HttpUtility.UrlEncode(userName)}";

            using (StringContent content = new StringContent(string.Empty))
            {
                using (HttpResponseMessage response = await GetClient().PostAsync(uri, content, cancellationToken))
                {
                    await CheckResponseForErrors(response);
                }
            }
        }

        public async Task<GetUsersResponse> GetUsers(CancellationToken cancellationToken = default)
        {
            string uri = "api/auth/users";

            using (HttpResponseMessage response = await GetClient().GetAsync(uri, cancellationToken))
            {
                await CheckResponseForErrors(response);

                return await Deserialize<GetUsersResponse>(response, cancellationToken);
            }
        }

        public async Task<UserInfo> GetUser(string userName, CancellationToken cancellationToken = default)
        {
            string uri = $"api/auth/users/{HttpUtility.UrlEncode(userName)}";

            using (HttpResponseMessage response = await GetClient().GetAsync(uri, cancellationToken))
            {
                await CheckResponseForErrors(response);

                return await Deserialize<UserInfo>(response, cancellationToken);
            }
        }

        public async Task VerifyUser(string userName, CancellationToken cancellationToken = default)
        {
            string uri = $"api/auth/users/{HttpUtility.UrlEncode(userName)}/verify";

            using (StringContent content = new StringContent(string.Empty))
            {
                using (HttpResponseMessage response = await GetClient().PostAsync(uri, content, cancellationToken))
                {
                    await CheckResponseForErrors(response);
                }
            }
        }

        public async Task UpdateUserRoles(string userName, IEnumerable<string> roles, CancellationToken cancellationToken = default)
        {
            string roleString = string.Join(",", roles);
            string uri = $"api/auth/users/{HttpUtility.UrlEncode(userName)}/roles";
            uri = QueryHelpers.AddQueryString(uri, "roles", roleString);

            using (StringContent content = new StringContent(string.Empty))
            {
                using (HttpResponseMessage response = await GetClient().PostAsync(uri, content, cancellationToken))
                {
                    await CheckResponseForErrors(response);
                }
            }
        }

        public async Task OverridePassword(string userName, string password, CancellationToken cancellationToken = default)
        {
            string uri = $"api/auth/users/{HttpUtility.UrlDecode(userName)}/password";
            uri = QueryHelpers.AddQueryString(uri, "password", password);

            using (StringContent content = new StringContent(string.Empty))
            {
                using (HttpResponseMessage response = await GetClient().PostAsync(uri, content, cancellationToken))
                {
                    await CheckResponseForErrors(response);
                }
            }
        }

        public async Task OverridePassword(string userName, string originalPassword, string password, CancellationToken cancellationToken = default)
        {
            string uri = $"api/auth/users/{HttpUtility.UrlDecode(userName)}/password";
            uri = QueryHelpers.AddQueryString(uri, new Dictionary<string, string> { { "originalPassword", originalPassword }, { "password", password } });

            using (StringContent content = new StringContent(string.Empty))
            {
                using (HttpResponseMessage response = await GetClient().PutAsync(uri, content, cancellationToken))
                {
                    await CheckResponseForErrors(response);
                }
            }
        }

        public async Task LockUser(string userName, bool lockout, CancellationToken cancellationToken = default)
        {
            string uri = $"api/auth/users/{HttpUtility.UrlDecode(userName)}/lock";
            uri = QueryHelpers.AddQueryString(uri, "lockout", lockout.ToString(CultureInfo.InvariantCulture));

            using (StringContent content = new StringContent(string.Empty))
            {
                using (HttpResponseMessage response = await GetClient().PutAsync(uri, content, cancellationToken))
                {
                    await CheckResponseForErrors(response);
                }
            }
        }
    }
}
