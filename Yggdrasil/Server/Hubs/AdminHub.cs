using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Yggdrasil.Identity;

namespace Yggdrasil.Server.Hubs
{
    /// <summary>
    /// SignalR Hub for user-based events
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AllowAnonymous]
    public sealed class AdminHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole(Roles.ManageUsers))
                await Groups.AddToGroupAsync(Context.ConnectionId, Roles.ManageUsers);
            if (Context.User.IsInRole(Roles.ManageUserPermissions))
                await Groups.AddToGroupAsync(Context.ConnectionId, Roles.ManageUserPermissions);

            await base.OnConnectedAsync();
        }
    }
}
