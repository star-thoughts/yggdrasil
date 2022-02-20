namespace Yggdrasil.Auditing
{
    /// <summary>
    /// Contains the result of a query to get audit records
    /// </summary>
    public sealed class AuditResult
    {
        /// <summary>
        /// Gets or sets the resulting records
        /// </summary>
        public AuditRecord[]? Records { get; set; }
    }
}
