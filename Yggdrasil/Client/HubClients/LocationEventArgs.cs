using Yggdrasil.Models.Locations;

namespace Yggdrasil.Client.HubClients
{
    /// <summary>
    /// Event arguments for a location being added to the campaign
    /// </summary>
    public class LocationEventArgs : BaseEventArgs
    {
        /// <summary>
        /// Constructs a new <see cref="LocationEventArgs"/>
        /// </summary>
        /// <param name="editingUser">User that added the location</param>
        /// <param name="location">Location that was added</param>
        public LocationEventArgs(string editingUser, Location location)
            : base(editingUser)
        {
            Location = location;
        }

        /// <summary>
        /// Gets the added location
        /// </summary>
        public Location Location { get; init; }
    }
}