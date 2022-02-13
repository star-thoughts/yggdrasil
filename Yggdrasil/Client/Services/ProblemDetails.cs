using System.Collections.Generic;
using System.Net;

namespace Yggdrasil.Client.Services
{
    /// <summary>
    /// Contains information about application/problem+json error details
    /// </summary>
    public sealed class ProblemDetails
    {
        /// <summary>
        /// Gets or sets human-readable explanation of this occurence of the problem
        /// </summary>
        public string Detail { get; set; }
        /// <summary>
        /// Gets or sets extended details about the problem
        /// </summary>
        public IDictionary<string, object> Extensions { get; set; }
        /// <summary>
        /// Gets or sets a URI refrence that identifies the specific occurrence of the problem
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// Gets or sets the HTTP status code of the problem
        /// </summary>
        public HttpStatusCode Status { get; set; }
        /// <summary>
        /// Gets or sets a short, human-readable summary of the problem
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets a URI reference that identifies the problem type.
        /// </summary>
        public string Type { get; set; }
    }
}