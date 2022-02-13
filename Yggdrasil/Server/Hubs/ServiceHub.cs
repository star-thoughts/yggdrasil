using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Yggdrasil.Server.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AllowAnonymous]
    public sealed class ServiceHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string campaignID = GetCampaignID();

            if (!string.IsNullOrWhiteSpace(campaignID))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, campaignID, Context.ConnectionAborted);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string campaignID = GetCampaignID();

            if (!string.IsNullOrWhiteSpace(campaignID))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, campaignID, Context.ConnectionAborted);
            }

            await base.OnDisconnectedAsync(exception);
        }


        /// <summary>
        /// Gets the ID of the campaign from the user's claims
        /// </summary>
        /// <returns>ID of the campaign, or null or empty if one is not found</returns>
        string GetCampaignID()
        {
            if (Context.User.Identity is ClaimsIdentity identity)
            {
                return identity.Claims.FirstOrDefault(p => string.Equals(p.Type, "campaign", StringComparison.Ordinal))?.Value;
            }
            return null;
        }
    }
}
