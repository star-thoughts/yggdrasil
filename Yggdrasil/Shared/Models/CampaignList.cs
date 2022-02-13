using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yggdrasil.Models
{
    /// <summary>
    /// List of camapigns in the system
    /// </summary>
    public sealed class CampaignList
    {
        /// <summary>
        /// Gets or sets a collection of campaigns in the system
        /// </summary>
        public CampaignOverview[] Campaigns { get; set; }
    }
}
