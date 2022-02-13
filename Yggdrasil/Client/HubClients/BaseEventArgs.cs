using System;

namespace Yggdrasil.Client.HubClients
{
    /// <summary>
    /// Base event args
    /// </summary>
    public class BaseEventArgs : EventArgs
    {
        /// <summary>
        /// Constructs a new <see cref="BaseEventArgs"/>
        /// </summary>
        /// <param name="editingUser">User that triggered the event</param>
        public BaseEventArgs(string editingUser)
        {
            EditingUser = editingUser;
        }

        /// <summary>
        /// Gets or sets the user that triggered the event
        /// </summary>
        public string EditingUser { get; set; }
    }
}
