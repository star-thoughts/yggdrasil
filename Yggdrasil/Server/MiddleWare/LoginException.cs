using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Server.Identity;

namespace Yggdrasil.Server.MiddleWare
{
    public class LoginException : Exception
    {
        public LoginException(string message)
            : base(message)
        {
        }

        public LoginException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
