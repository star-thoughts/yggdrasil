using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yggdrasil.Models
{
    public sealed class GetCampaignResult
    {
        /// <summary>
        /// Gets the campaign name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets a short description, or overview of the campaign
        /// </summary>
        public string Overview { get; set; }
        /// <summary>
        /// Gets a list of user information
        /// </summary>
        public IEnumerable<CampaignUserData> Users { get; set; }
    }
}
