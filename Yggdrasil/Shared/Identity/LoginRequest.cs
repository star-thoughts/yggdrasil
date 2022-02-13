namespace Yggdrasil.Identity
{
    /// <summary>
    /// Contains login info for a login request
    /// </summary>
    public sealed class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
