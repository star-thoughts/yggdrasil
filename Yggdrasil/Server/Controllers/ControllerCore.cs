using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using Yggdrasil.Server.MiddleWare;
using Yggdrasil.Server.Storage;

namespace Yggdrasil.Server.Controllers
{
    public abstract class ControllerCore : ControllerBase
    {
        #region Helpers
        /// <summary>
        /// Gets the ID of the campaign from the user's claims
        /// </summary>
        /// <exception cref="LoginException">Identity information is missing from the request, or the campaign is missing</exception>
        /// <returns>ID of the campaign, or null or empty if one is not found</returns>
        protected string GetCampaignID()
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                string? campaignID = identity.Claims.FirstOrDefault(p => string.Equals(p.Type, "campaign", StringComparison.Ordinal))?.Value;
                if (string.IsNullOrWhiteSpace(campaignID))
                    throw new LoginException("Campaign details were not included in a claim.");

                return campaignID;
            }
            throw new LoginException("A campaign claim must be sent");
        }

        /// <summary>
        /// Gets the user's name from the user's claims
        /// </summary>
        /// <returns></returns>
        /// <exception cref="LoginException">Identity information is missing from the request</exception>
        protected string GetUserName()
        {
            return HttpContext?.User?.Identity?.Name ?? throw new LoginException("A user claim must be sent");
        }
        #endregion
    }
}
