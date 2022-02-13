namespace Yggdrasil.Client.HubClients
{
    public class UserRemovedEventArgs : BaseEventArgs
    {
        public UserRemovedEventArgs(string editingUser, string userName) : base(editingUser)
        {
        }

        /// <summary>
        /// Gets the ID of the user that was removed
        /// </summary>
        public string UserName { get; }
    }
}