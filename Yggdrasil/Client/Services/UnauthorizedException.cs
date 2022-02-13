using System;
using System.Net;
using System.Runtime.Serialization;

namespace Yggdrasil.Client.Services
{
    [Serializable]
    public class UnauthorizedException : ProblemException
    {
        public UnauthorizedException()
            : base(new ProblemDetails()
            {
                Title = "Unauthorized Access",
                Detail = "You are not authorized to access the requested resource",
                Status = HttpStatusCode.BadRequest,
            })
        {
        }
        public UnauthorizedException(ProblemDetails details)
            : base(details)
        {
        }

        protected UnauthorizedException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}