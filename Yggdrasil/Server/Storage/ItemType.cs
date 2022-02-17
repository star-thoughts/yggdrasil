namespace Yggdrasil.Server.Storage
{
    /// <summary>
    /// Enumeration of types of items in the database
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// Unknown or unset type
        /// </summary>
        Unknown,
        /// <summary>
        /// Campaign item
        /// </summary>
        Campaign,
        /// <summary>
        /// Player character
        /// </summary>
        PlayerCharacter,
        /// <summary>
        /// Campaign location
        /// </summary>
        Location,
    }
}