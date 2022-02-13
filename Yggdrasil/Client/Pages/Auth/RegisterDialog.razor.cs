using Fiction.Controls.Dialog;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yggdrasil.Client.Pages.Exceptions;
using Yggdrasil.Client.Services;
using Yggdrasil.Identity;

namespace Yggdrasil.Client.Pages.Auth
{
    /// <summary>
    /// Dialog for registering a user
    /// </summary>
    public sealed partial class RegisterDialog
    {
        [Inject]
        IAuthService AuthService { get; set; }

        Dialog Dialog;
        ExceptionDialog ExceptionDialog;

        string UserName { get; set; }
        string Password { get; set; }
        string PasswordCompare { get; set; }

        bool ArePasswordsSame => string.Equals(Password, PasswordCompare, StringComparison.Ordinal);
        bool IsValid => ArePasswordsSame && !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
        string Warning { get; set; }

        public Task Show()
        {
            return Dialog.Show();
        }

        async Task OnAccept()
        {
            try
            {
                RegisterResult result = await AuthService.Register(UserName, Password);
                if (!result.IsSuccess)
                    Warning = result.Errors.FirstOrDefault();
                else
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
