using Radzen;
using System.Threading.Tasks;

namespace Yggdrasil.Client.Pages.Components
{
    /// <summary>
    /// Extension methods and helpers for displaying dialogs
    /// </summary>
    public static class DialogHelpers
    {
        static DialogOptions MessageBoxOptions = new DialogOptions() { CloseDialogOnEsc = true };

        /// <summary>
        /// Opens a new dialog box to display a message
        /// </summary>
        /// <param name="service">Service for handling dialog operations</param>
        /// <param name="title">Title of the message box to display</param>
        /// <param name="message">Message to display within the dialog box</param>
        /// <param name="buttons">Types of buttons to display</param>
        /// <returns>Which button the user clicked</returns>
        public static async Task<MessageBoxResult> MessageBoxAsync(this DialogService service, string title, string message, MessageBoxType buttons)
        {
            dynamic result = await service.OpenAsync<MessageBoxDialog>(title,
                Components.MessageBoxOptions.Create(message, buttons),
                MessageBoxOptions);

            if (result == null)
            {
                switch (buttons)
                {
                    case MessageBoxType.OkCancel:
                    case MessageBoxType.YesNoCancel:
                        result = MessageBoxResult.Cancel;
                        break;
                    case MessageBoxType.Close:
                        result = MessageBoxResult.Close;
                        break;
                    case MessageBoxType.YesNo:
                        result = MessageBoxResult.No;
                        break;
                }
            }

            return result;
        }
    }
}
