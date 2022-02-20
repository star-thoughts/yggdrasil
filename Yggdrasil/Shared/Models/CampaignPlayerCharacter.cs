namespace Yggdrasil.Models
{
    /// <summary>
    /// Contains information about a player character in the campaign
    /// </summary>
    public sealed class CampaignPlayerCharacter
    {
        /// <summary>
        /// Gets or sets the ID of this character
        /// </summary>
        public string? ID { get; set; }
        /// <summary>
        /// Gets or sets the username of the player, can be null if not claimed
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// Gets or sets the name of the character
        /// </summary>
        public string? Name { get; set; }
    }
}