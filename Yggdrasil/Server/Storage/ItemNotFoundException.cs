using System;
using System.Runtime.Serialization;

namespace Yggdrasil.Server.Storage
{
    /// <summary>
    /// Exception that is thrown when an item expected in the database does not exist
    /// </summary>
#pragma warning disable CA1032 // Implement standard exception constructors
    public sealed class ItemNotFoundException : Exception
    {
        /// <summary>
        /// Constructs a new <see cref="ItemNotFoundException"/>
        /// </summary>
        /// <param name="type">Type of item that was not found</param>
        /// <param name="id">ID of the item that was not found</param>
        public ItemNotFoundException(ItemType type, string id)
            : base($"An item of type {type} ({id}) could not be found.")
        {
            ItemType = type;
            ItemID = id;
        }

        /// <summary>
        /// Constructs a new <see cref="ItemNotFoundException"/>, deserializing the details
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ItemNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ItemType = (ItemType)info.GetValue("itemType", typeof(ItemType));
            ItemID = (string)info.GetValue("itemID", typeof(string));
        }

        /// <summary>
        /// Gets the type of item that was not found
        /// </summary>
        public ItemType ItemType { get; }
        /// <summary>
        /// Gets the ID of the item that was not found
        /// </summary>
        public string ItemID { get; }

        /// <summary>
        /// Serializes the exception
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("itemType", ItemType);
            info.AddValue("itemID", ItemID);
        }
    }
#pragma warning restore CA1032 // Implement standard exception constructors
}
