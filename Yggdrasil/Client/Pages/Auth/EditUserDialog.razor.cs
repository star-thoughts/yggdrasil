using Fiction.Controls.Dialog;
using Microsoft.AspNetCore.Components;
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
    public sealed partial class EditUserDialog : IDisposable
    {
        [Inject]
        IAuthService AuthService { get; set; }
        [CascadingParameter]
        LocalState LocalState { get; set; }
        UserInfo User { get; set; }
        string Pass1 { get; set; }
        string Pass2 { get; set; }
        List<string> _roles = new List<string>();
        bool _busy;

        Dialog Dialog;
        ExceptionDialog ExceptionDialog;
        MessageBox MessageBox;

        public Task Show(UserInfo user)
        {
            User = user;

            if (User == null)
            {
                return Task.CompletedTask;
            }

            return Dialog.Show();
        }

        void DialogOpened()
        {
            _roles = User.Roles.ToList();

            if (LocalState?.AdminHub != null)
            {
                LocalState.AdminHub.UserUpdated += HubClient_UserUpdated;
                LocalState.AdminHub.UserRemoved += HubClient_UserRemoved;
            }
        }

        void DialogClosing()
        {
            if (LocalState?.AdminHub != null)
            {
                LocalState.AdminHub.UserUpdated -= HubClient_UserUpdated;
                LocalState.AdminHub.UserRemoved -= HubClient_UserRemoved;
            }
        }

        private async void HubClient_UserRemoved(object sender, UserRemovedEventArgs e)
        {
            if (string.Equals(User.UserName, e.UserName, StringComparison.OrdinalIgnoreCase))
                await NotifyUserChangedAndClose();
        }

        private async void HubClient_UserUpdated(object sender, UserUpdatedEventArgs e)
        {
            if (string.Equals(e.User.UserName, User.UserName, StringComparison.OrdinalIgnoreCase))
                await NotifyUserChangedAndClose();
        }

        async Task NotifyUserChangedAndClose()
        {
            //  If we're busy, it's either us doing it or it's too late anyway
            if (!_busy)
            {
                await MessageBox.ShowMessage("User Changed or Removed", "The current user has been changed or removed and cannot be edited right now.");
                await Dialog.CloseDialog(false);
            }
        }

        bool IsRoleActive(string role)
        {
            return _roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        void ToggleRole(string role, bool selected)
        {
            if (selected && !_roles.Contains(role))
                _roles.Add(role);
            if (!selected)
                _roles.Remove(role);
        }

        async Task OnAccept()
        {
            _busy = true;
            try
            {
                await InvokeAsync(StateHasChanged);
                //  If the list of roles do not match, update them
                if (_roles.Any(p => !User.Roles.Contains(p, StringComparer.Ordinal)) || User.Roles.Any(p => !_roles.Contains(p, StringComparer.Ordinal)))
                    await AuthService.UpdateUserRoles(User.UserName, _roles);
                if (string.Equals(Pass1, Pass2, StringComparison.Ordinal) && !string.IsNullOrWhiteSpace(Pass1))
                    await AuthService.OverridePassword(User.UserName, Pass1);

                await Dialog.CloseDialog(true);
            }
            catch (Exception exc)
            {
                await ExceptionDialog.Show(exc);
            }
            finally
            {
                _busy = false;
            }
        }

        async Task OnCancel()
        {
            await Dialog.CloseDialog(false);
        }

        public void Dispose()
        {
            if (LocalState?.AdminHub != null)
            {
                LocalState.AdminHub.UserUpdated -= HubClient_UserUpdated;
                LocalState.AdminHub.UserRemoved -= HubClient_UserRemoved;
            }
        }
    }
}
