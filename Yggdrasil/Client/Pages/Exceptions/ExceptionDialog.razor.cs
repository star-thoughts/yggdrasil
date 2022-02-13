using Fiction.Controls.Dialog;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Yggdrasil.Client.Pages.Exceptions
{
    public sealed partial class ExceptionDialog
    {
        /// <summary>
        /// Gets or sets extra class features
        /// </summary>
        [Parameter]
        public string Class { get; set; }
        /// <summary>
        /// Gets the exception that is being displayed
        /// </summary>
        public Exception _exception { get; set; }
        bool ShowDev => false;
        Dialog Dialog;

        /// <summary>
        /// Shows the given exception in a dialog
        /// </summary>
        /// <param name="exc">Exception to display</param>
        /// <returns>Task for asynchronous completion</returns>
        public Task Show(Exception exc)
        {
            _exception = exc;
            return Dialog.Show();
        }

        void OnAccept()
        {
            Dialog.CloseDialog(true);
        }

        string GetClass()
        {
            return $"exception-details {Class}";
        }
    }
}
