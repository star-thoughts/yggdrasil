using System.Collections.Generic;

namespace Yggdrasil.Identity
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public Dictionary<string, string> RoleMap { get; set; }
    }
}
