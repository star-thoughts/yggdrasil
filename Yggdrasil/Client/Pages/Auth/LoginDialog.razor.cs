using Fiction.Controls.Dialog;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Yggdrasil.Client.Pages.Exceptions;
using Yggdrasil.Client.Services;
using Yggdrasil.Identity;

namespace Yggdrasil.Client.Pages.Auth
{
    public sealed partial class LoginDialog
    {
        [Inject]
        IAuthService AuthService { get; set; }

        Dialog Dialog;
        ExceptionDialog ExceptionDialog;

        string UserName { get; set; }
        string Password { get; set; }
        bool IsValid => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);

        public Task Show()
        {
            return Dialog.Show();
        }

        async Task OnAccept()
        {
            try
            {
                LoginResponse result = await AuthService.Login(UserName, Password);
                await Dialog.CloseDialog(true);
            }
            catch (Exception exc)
            {
                await ExceptionDialog.Show(exc);
            }

            await InvokeAsync(StateHasChanged);
        }

        void OnCancel()
        {
            Dialog.CloseDialog(false);
        }
    }
}
