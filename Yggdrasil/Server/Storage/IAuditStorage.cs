using System.Threading;
using System.Threading.Tasks;
using Yggdrasil.Auditing;

namespace Yggdrasil.Server.Storage
{
    /// <summary>
    /// Storage interface for auditing
    /// </summary>
    public interface IAuditStorage
    {
        /// <summary>
        /// Adds an audit record to the audit database
        /// </summary>
        /// <param name="record">Audit record to add</param>
        /// <param name="cancellationToken">Token for cancelling the operation</param>
        /// <returns>Task for asynchronous completion</returns>
        Task AddAuditRecord(AuditRecord record, CancellationToken cancellationToken = default);
    }
}
