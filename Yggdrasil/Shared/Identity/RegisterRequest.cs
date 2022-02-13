using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yggdrasil.Identity
{
    /// <summary>
    /// Contains a request to register an account
    /// </summary>
    public sealed class RegisterRequest
    {
        /// <summary>
        /// Gets or sets the username being registered
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the password being set
        /// </summary>
        public string Password { get; set; }
    }
}
