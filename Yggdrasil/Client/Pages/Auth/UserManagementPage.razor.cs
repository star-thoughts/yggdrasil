using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Client.HubClients;
using Yggdrasil.Client.Models;
using Yggdrasil.Client.Pages.Exceptions;
using Yggdrasil.Client.Services;
using Yggdrasil.Identity;

namespace Yggdrasil.Client.Pages.Auth
{
    public sealed partial class UserManagementPage : IDisposable
    {
        [Inject]
        IAuthService AuthService { get; set; }
        [Inject]
        AuthenticationStateProvider AuthProvider { get; set; }
        [CascadingParameter]
        LocalState Globals { get; set; }

        bool _loading = true;
        List<UserInfo> Users;
        UserInfo SelectedUser;

        ExceptionDialog ExceptionDialog;
        EditUserDialog EditUserDialog;

        protected override async Task OnInitializedAsync()
        {
            AuthProvider.AuthenticationStateChanged += AuthProvider_AuthenticationStateChanged;
            if ((await AuthProvider.GetAuthenticationStateAsync()).User.IsInRole(Roles.ManageUsers))
            {
                await Connect();
            }

            await base.OnInitializedAsync();
        }

        private async Task Connect()
        {
            _loading = true;
            try
            {
                Users = GetSortedUserList((await AuthService.GetUsers()).Users);
            }
            catch (Exception exc)
            {
                await ExceptionDialog.Show(exc);
            }
            finally
            {
                _loading = false;
            }

            Globals.AdminHub.UserAdded += AdminHub_UserUpdated;
            Globals.AdminHub.UserUpdated += AdminHub_UserUpdated;
            Globals.AdminHub.UserRemoved += AdminHub_UserRemoved;
        }

        private async void AuthProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            if ((await task).User.IsInRole(Roles.ManageUsers))
            {
                foreach (System.Security.Claims.Claim role in (await task).User.Claims)
                    Console.WriteLine(role.Value);
                await Connect();
            }
            await InvokeAsync(StateHasChanged);
        }

        private List<UserInfo> GetSortedUserList(IEnumerable<UserInfo> users)
        {
            return users.OrderBy(p => p.IsVerified)
                .ThenBy(p => p.UserName)
                .ToList();
        }

        private async void AdminHub_UserRemoved(object sender, UserRemovedEventArgs e)
        {
            UserInfo user = Users.FirstOrDefault(p => string.Equals(p.UserName, e.UserName, StringComparison.OrdinalIgnoreCase));
            Users.Remove(user);

            await InvokeAsync(StateHasChanged);
        }

        private async void AdminHub_UserUpdated(object sender, UserUpdatedEventArgs e)
        {
            UserInfo existing = Users.FirstOrDefault(p => string.Equals(p.UserName, e.User.UserName, StringComparison.OrdinalIgnoreCase));
            Users.Remove(existing);
            Users.Add(e.User);
            if (ReferenceEquals(existing, SelectedUser))
                SelectedUser = e.User;

            Users = GetSortedUserList(Users);

            await InvokeAsync(StateHasChanged);
        }

        Task EditUser()
        {
            if (SelectedUser != null)
            {
                UserInfo user = SelectedUser;
                return EditUserDialog.Show(user);
            }
            return Task.CompletedTask;
        }

        void RemoveUser()
        {
        }

        async Task VerifyUser()
        {
            UserInfo user = SelectedUser;
            if (user != null)
            {
                try
                {
                    await AuthService.VerifyUser(user.UserName);
                }
                catch (Exception exc)
                {
                    await ExceptionDialog.Show(exc);
                }
            }
        }

        async Task LockUser()
        {
            UserInfo user = SelectedUser;
            if (user != null)
            {
                try
                {
                    await AuthService.LockUser(user.UserName, !user.IsLocked);
                }
                catch (Exception exc)
                {
                    await ExceptionDialog.Show(exc);
                }
            }
        }
        public void Dispose()
        {
            Globals.AdminHub.UserAdded -= AdminHub_UserUpdated;
            Globals.AdminHub.UserUpdated -= AdminHub_UserUpdated;
            Globals.AdminHub.UserRemoved -= AdminHub_UserRemoved;
            AuthProvider.AuthenticationStateChanged -= AuthProvider_AuthenticationStateChanged;
        }
    }
}
