using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Identity;
using Yggdrasil.SignalR;

namespace Yggdrasil.Server.Hubs
{
    /// <summary>
    /// Extension methods for calling admin hub methods
    /// </summary>
    public static class AdminHubExtensions
    {
        public static Task UserAdded(this IHubContext<AdminHub> hub, string editingUser, UserInfo user)
        {
            if (hub == null)
                throw new ArgumentNullException(nameof(hub));
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return hub.Clients.Group(Roles.ManageUsers).SendAsync(AdminHubMethods.UserAdded, editingUser, user);
        }

        public static Task UserUpdated(this IHubContext<AdminHub> hub, string editingUser, UserInfo user)
        {
            if (hub == null)
                throw new ArgumentNullException(nameof(hub));
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return hub.Clients.Group(Roles.ManageUsers).SendAsync(AdminHubMethods.UserUpdated, editingUser, user);
        }

        public static Task UserRemoved(this IHubContext<AdminHub> hub, string editingUser, string userID)
        {
            if (hub == null)
                throw new ArgumentNullException(nameof(hub));
            if (string.IsNullOrWhiteSpace(userID))
                throw new ArgumentNullException(nameof(userID));

            return hub.Clients.Group(Roles.ManageUsers).SendAsync(AdminHubMethods.UserRemoved, editingUser, userID);
        }
    }
}
