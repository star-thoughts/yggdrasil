using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Yggdrasil.Client.Identity
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        internal static string JwtToken { get; set; }

        public ApiAuthenticationStateProvider()
        {
        }
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (string.IsNullOrWhiteSpace(JwtToken) || string.Equals(JwtToken, "null", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            }

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(JwtToken).ToArray(), "jwt"))));
        }

        public Task MarkUserAsAuthenticated(string jwt)
        {
            ClaimsPrincipal authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(jwt).ToArray(), "jwt"));
            JwtToken = jwt;
            Task<AuthenticationState> authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
            return Task.CompletedTask;
        }

        public void MarkUserAsLoggedOut()
        {
            ClaimsPrincipal anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            JwtToken = null;
            Task<AuthenticationState> authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public static JwtSecurityToken GetSecurityToken(string jwt)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jsonToken = handler.ReadToken(jwt) as JwtSecurityToken;
            return jsonToken;
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            JwtSecurityToken jsonToken = GetSecurityToken(jwt);

            if (jsonToken != null)
            {
                return jsonToken.Claims;
            }
            return Array.Empty<Claim>();
        }
    }
}
