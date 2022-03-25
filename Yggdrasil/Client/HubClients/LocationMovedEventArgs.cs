using Yggdrasil.Models.Locations;

namespace Yggdrasil.Client.HubClients
{
    /// <summary>
    /// Event arguments for locations being moved to different parents
    /// </summary>
    public sealed class LocationMovedEventArgs : BaseEventArgs
    {
        /// <summary>
        /// Constructs a new <see cref="LocationMovedEventArgs"/>
        /// </summary>
        /// <param name="editingUser">User that edited the locations</param>
        /// <param name="locations">Information about the locations that were moved</param>
        public LocationMovedEventArgs(string editingUser, LocationsMoved locations)
            : base(editingUser)
        {
            LocationsMoved = locations;
        }

        /// <summary>
        /// Gets the location information for the moved locations
        /// </summary>
        public LocationsMoved LocationsMoved { get; init; }
    }
}