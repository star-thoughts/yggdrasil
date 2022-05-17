namespace Yggdrasil.Client.Pages.Components
{
    /// <summary>
    /// Defines the possible types of message boxes available
    /// </summary>
    public enum MessageBoxType
    {
        /// <summary>
        /// Presents the dialog box with an OK button and a Cancel button
        /// </summary>
        OkCancel = 0,
        /// <summary>
        /// Presents the dialog box with a Yes button and a No button
        /// </summary>
        YesNo = 1,
        /// <summary>
        /// Presents the dialog box with a Yes button, a No button, and a Cancel button
        /// </summary>
        YesNoCancel = 2,
        /// <summary>
        /// Presents the dialog box with a Close button
        /// </summary>
        Close = 3,
    }
}