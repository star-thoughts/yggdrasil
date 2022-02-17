using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using Yggdrasil.Auditing;

namespace Yggdrasil.Server.Storage.Mongo
{
    public sealed class MongoAuditRecord
    {
        /// <summary>
        /// Creates an <see cref="AuditRecord"/> from this database entry
        /// </summary>
        /// <returns>Record of entry</returns>
        public AuditRecord ToAuditRecord()
        {
            return new AuditRecord()
            {
                RecordType = RecordType,
                Action = Action,
                Variables = Variables,
                DateTime = DateTime,
                User = User,
                Correlation = CorrelationId,
            };
        }

        /// <summary>
        /// Creates a new audit record database entry
        /// </summary>
        /// <param name="auditRecord">Record to create entry from</param>
        /// <returns>Record database entry</returns>
        public static MongoAuditRecord FromAuditRecord(AuditRecord auditRecord)
        {
            if (auditRecord == null)
                throw new ArgumentNullException(nameof(auditRecord));

            return new MongoAuditRecord()
            {
                RecordType = auditRecord.RecordType,
                Action = auditRecord.Action,
                Variables = auditRecord.Variables,
                DateTime = auditRecord.DateTime,
                User = auditRecord.User,
                CorrelationId = auditRecord.RecordType,
            };
        }

        /// <summary>
        /// Gets or sets the record ID
        /// </summary>
        [BsonId]
        public string? ID { get; set; }
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
        /// Gets or sets a correlation ID used to tie records to a single action
        /// </summary>
        public string? CorrelationId { get; set; }
    }
}
