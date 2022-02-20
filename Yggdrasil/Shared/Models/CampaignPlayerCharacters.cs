using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yggdrasil.Models
{
    public sealed class CampaignPlayerCharacters
    {
        /// <summary>
        /// Gets a collection of players in the campaign
        /// </summary>
        public IEnumerable<CampaignPlayerCharacter>? Characters { get; set; }
    }
}
