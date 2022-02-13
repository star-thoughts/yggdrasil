using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yggdrasil.SignalR
{
    /// <summary>
    /// Contains method names used by the Service hub
    /// </summary>
    public static class ServiceHubMethods
    {
        public const string CampaignAdded = nameof(CampaignAdded);
        public const string CampaignRemoved = nameof(CampaignRemoved);
        public const string CampaignUpdated = nameof(CampaignUpdated);
        public const string PCAdded = nameof(PCAdded);
        public const string PCUpdated = nameof(PCUpdated);
        public const string PCRemoved = nameof(PCRemoved);
    }
}
