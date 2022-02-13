namespace Yggdrasil.Auditing
{
    /// <summary>
    /// Enumeration of audit record sub-types
    /// </summary>
    public enum AuditAction
    {
        /// <summary>
        /// No subtype
        /// </summary>
        None,
        /// <summary>
        /// Item was created
        /// </summary>
        Created,
        /// <summary>
        /// Item was edited
        /// </summary>
        Edited,
        /// <summary>
        /// Item was removed
        /// </summary>
        Removed
    }
}