using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Identity
{
    /// <summary>
    /// Contains login info for a login request
    /// </summary>
    public sealed class LoginRequest
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
