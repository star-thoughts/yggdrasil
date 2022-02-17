using Microsoft.AspNetCore.Identity;
using System;

namespace Yggdrasil.Server.Identity
{
    /// <summary>
    /// Exception throws when an attempt to lock or unlock an account fails
    /// </summary>
    public class LockoutFailedException : Exception
    {
        public LockoutFailedException(IdentityResult result)
            : base("Lockout set/reset attempt failed.")
        {
            Result = result;
        }

        /// <summary>
        /// Gets the state of the attempt
        /// </summary>
        public IdentityResult? Result { get; }

        public LockoutFailedException()
        {
        }

        public LockoutFailedException(string message) : base(message)
        {
        }

        public LockoutFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
