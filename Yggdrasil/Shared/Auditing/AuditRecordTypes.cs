namespace Yggdrasil.Auditing
{
    /// <summary>
    /// Constains the commonlyused audit record types
    /// </summary>
    public static class AuditRecordTypes
    {
        /// <summary>
        /// User audit record
        /// </summary>
        public const string User = nameof(User);
        /// <summary>
        /// Campaign audit record
        /// </summary>
        public const string Campaign = nameof(Campaign);
        /// <summary>
        /// Location audit record
        /// </summary>
        public const string Location = nameof(Location);
    }
}
