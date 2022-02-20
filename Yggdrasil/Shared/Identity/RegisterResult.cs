namespace Yggdrasil.Identity
{
    public sealed class RegisterResult
    {
        public bool IsSuccess { get; set; }
        public string[]? Errors { get; set; }
    }
}
