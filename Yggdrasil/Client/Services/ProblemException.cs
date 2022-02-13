using System;
using System.Net;
using System.Runtime.Serialization;

namespace Yggdrasil.Client.Services
{
    [Serializable]
    public class ProblemException : Exception
    {
        public ProblemException(ProblemDetails details)
            : base(details.Detail)
        {
            StatusCode = (HttpStatusCode)Convert.ToInt32(details.Status);
            Title = details.Title;
        }

        protected ProblemException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the HTTP Status code
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
        /// <summary>
        /// Gets the error details
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// Gets error instance information from the server
        /// </summary>
        public string Instance { get; private set; }
    }
}
