using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yggdrasil.Models
{
    /// <summary>
    /// Short details about a campaign
    /// </summary>
    public sealed class CampaignOverview
    {
        /// <summary>
        /// Gets or sets the ID of this campaign
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Gets or sets the name of the campaign
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets a short description for the campaign
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// Gets or sets the username for the dungeon master
        /// </summary>
        public string DungeonMaster { get; set; }
    }
}
