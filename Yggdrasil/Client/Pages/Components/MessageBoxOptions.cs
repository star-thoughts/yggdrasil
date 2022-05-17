using System.Collections.Generic;

namespace Yggdrasil.Client.Pages.Components
{
    /// <summary>
    /// Options to use when opening a <see cref="MessageBoxDialog"/>
    /// </summary>
    public sealed class MessageBoxOptions
    {
        /// <summary>
        /// Private constructor to prevent it from being constructed.  Use <see cref="Create(string, MessageBoxType)"/> instead.
        /// </summary>
        MessageBoxOptions()
        {
        }

        /// <summary>
        /// Gets or sets the message to display to the user
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets the buttons to display to the user
        /// </summary>
        public MessageBoxType Buttons { get; set; }

        /// <summary>
        /// Creates message box options to display to the user
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public static Dictionary<string, object> Create(string message, MessageBoxType buttons)
        {
            MessageBoxOptions options = new MessageBoxOptions()
            {
                Message = message,
                Buttons = buttons,
            };

            return new Dictionary<string, object> { { "MessageBoxOptions", options } };
        }
    }
}
