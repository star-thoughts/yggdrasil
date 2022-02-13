namespace Yggdrasil.Client.HubClients
{
    public class ItemRemovedEventArgs : BaseEventArgs
    {
        public ItemRemovedEventArgs(string editingUser, string itemID)
            : base(editingUser)
        {
            ItemID = itemID;
        }

        /// <summary>
        /// Gets the ID of the capmaign that was removed
        /// </summary>
        public string ItemID { get; init; }
    }
}