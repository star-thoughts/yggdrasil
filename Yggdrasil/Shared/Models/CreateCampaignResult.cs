using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yggdrasil.Models
{
    /// <summary>
    /// Contains the result of the creation of a campaign
    /// </summary>
    public sealed class CreateCampaignResult
    {
        /// <summary>
        /// Gets or sets the ID of the campaign that was created
        /// </summary>
        public string? ID { get; set; }
    }
}
