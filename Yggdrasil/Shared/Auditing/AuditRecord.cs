using System;
using System.Collections.Generic;

namespace Yggdrasil.Auditing
{
    public sealed class AuditRecord
    {
        /// <summary>
        /// Gets or sets the ID of the item this record is for
        /// </summary>
        /// <remarks>
        /// This is the ID of the item of the type given in <see cref="RecordType"/>
        /// </remarks>
        public string? ItemID { get; set; }
        /// <summary>
        /// Type of audit record
        /// </summary>
        public string? RecordType { get; set; }
        /// <summary>
        /// Action performed in this audit record
        /// </summary>
        public AuditAction Action { get; set; }
        /// <summary>
        /// Gets the variables for this audit record
        /// </summary>
        public Dictionary<string, string>? Variables { get; set; }
        /// <summary>
        /// Gets the date/time of the audit record
        /// </summary>
        public DateTime DateTime { get; set; }
        /// <summary>
        /// Gets the user associated with the audit record, if any
        /// </summary>
        public string? User { get; set; }
        /// <summary>
        /// Gets or sets a value used to correlate multiple audit records to a single action
        /// </summary>
        public string? Correlation { get; set; }
    }
}
