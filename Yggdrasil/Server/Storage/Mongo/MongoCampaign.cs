using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Models;

namespace Yggdrasil.Server.Storage.Mongo
{
    /// <summary>
    /// Mongo storage record for a campaign
    /// </summary>
    public sealed class MongoCampaign
    {
        /// <summary>
        /// Gets or sets the ID of the campaign
        /// </summary>
        [BsonId]
        public ObjectId ID { get; set; }
        /// <summary>
        /// Gets or sets hte name of the campaign
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets a short description of the campaign
        /// </summary>
        public string Overview { get; set; }
        /// <summary>
        /// Gets or sets the username of the dungeon master
        /// </summary>
        public string DungeonMaster { get; set; }
        /// <summary>
        /// Gets or sets data about users in the campaign
        /// </summary>
        public CampaignUserData[] Users { get; set; } = Array.Empty<CampaignUserData>();
        /// <summary>
        /// Gets or sets a collection of player characters in the campaign
        /// </summary>
        public CampaignPlayerCharacter[] PlayerCharacters { get; set; } = Array.Empty<CampaignPlayerCharacter>();
    }
}
