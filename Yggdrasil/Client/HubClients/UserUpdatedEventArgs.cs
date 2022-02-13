using Yggdrasil.Identity;

namespace Yggdrasil.Client.HubClients
{
    public class UserUpdatedEventArgs : BaseEventArgs
    {
        public UserUpdatedEventArgs(string editingUser, UserInfo user) 
            : base(editingUser)
        {
            User = user;
        }

        /// <summary>
        /// Gets the user that was updated
        /// </summary>
        public UserInfo User { get; }
    }
}