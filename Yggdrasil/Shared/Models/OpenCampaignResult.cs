using System.Collections.Generic;

namespace Yggdrasil.Models
{
    /// <summary>
    /// Contains the results of opening a campaign
    /// </summary>
    public sealed class OpenCampaignResult
    {
        /// <summary>
        /// Gets the token to use for the campaign for the user
        /// </summary>
        public string? CamapignToken { get; set; }
    }
}
