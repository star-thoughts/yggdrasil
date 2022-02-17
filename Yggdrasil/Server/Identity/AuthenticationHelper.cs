using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Yggdrasil.Server.Configuration;

namespace Yggdrasil.Server.Identity
{
    class AuthenticationHelper
    {
        /// <summary>
        /// Creates a generic JWT for logging in to the system
        /// </summary>
        /// <param name="user"></param>
        /// <param name="configuration"></param>
        /// <param name="availableRoles"></param>
        /// <returns></returns>
        public static string GenerateJwtToken(ApplicationUser user, JwtConfiguration configuration, ApplicationRole[] availableRoles)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (configuration?.Key == null)
                throw new ArgumentNullException(nameof(configuration));

            Dictionary<string, string> roleMap = availableRoles.ToDictionary(p => p.Id.ToString(), p => p.Name);

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            foreach (string role in user.Roles)
            {
                if (roleMap.TryGetValue(role, out string? roleName))
                    claims.Add(new Claim(ClaimTypes.Role, roleName));
                else
                    claims.Add(new Claim(ClaimTypes.Role, role));
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.Key));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expires = DateTime.Now.AddDays(Convert.ToDouble(configuration.ExpireDays));

            JwtSecurityToken token = new JwtSecurityToken(
                configuration.Issuer,
                configuration.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Creates a campaign-specific JWT
        /// </summary>
        /// <param name="user"></param>
        /// <param name="configuration"></param>
        /// <param name="campaignRoles">ID of the campaign the login is good for</param>
        /// <param name="availableRoles"></param>
        /// <param name="campaignID">Roles for the user in the campaign</param>
        /// <returns></returns>
        public static string GenerateCampaignJwtToken(ApplicationUser user, JwtConfiguration configuration, string campaignID, ApplicationRole[] availableRoles, IEnumerable<string> campaignRoles)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (configuration?.Key == null)
                throw new ArgumentNullException(nameof(configuration));

            Dictionary<string, string> roleMap = availableRoles.ToDictionary(p => p.Id.ToString(), p => p.Name);

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("campaign", campaignID),
            };

            foreach (string role in user.Roles)
            {
                if (roleMap.TryGetValue(role, out string? roleName))
                    claims.Add(new Claim(ClaimTypes.Role, roleName));
                else
                    claims.Add(new Claim(ClaimTypes.Role, role));
            }
            //  Add the campapign specific roles
            foreach (string role in campaignRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.Key));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expires = DateTime.Now.AddDays(Convert.ToDouble(configuration.ExpireDays));

            JwtSecurityToken token = new JwtSecurityToken(
                configuration.Issuer,
                configuration.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
