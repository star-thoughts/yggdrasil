using System;
using System.Collections.Generic;
using System.Linq;
using Yggdrasil.Client.ViewModels;
using Yggdrasil.Models.Locations;

namespace Yggdrasil.Client.Pages.Components
{
    /// <summary>
    /// Extension methods that help deal with Root Maps
    /// </summary>
    public static class RootMapExtensions
    {
        /// <summary>
        /// Creates a root map list out of a list of locations
        /// </summary>
        /// <param name="locations">List of locations to create a map from</param>
        /// <returns>List of root map items</returns>
        public static IEnumerable<RootMapItem> ToRootMap(this IEnumerable<LocationListItem> locations)
        {
            return locations.Select(p => new RootMapItem() { AncestorId = p.Id, AncestorName = p.Name });
        }

        /// <summary>
        /// Creates a root map list out of a location, using its ancestor path
        /// </summary>
        /// <param name="locations">List of locations to create a map from</param>
        /// <returns>List of root map items</returns>
        public static IEnumerable<RootMapItem> ToRootMap(this LocationViewModel location)
        {
            IEnumerable<RootMapItem> items = location?.Ancestors?.Reverse().ToRootMap() ?? Array.Empty<RootMapItem>();
            items = new RootMapItem[] { new RootMapItem() { AncestorId = null, AncestorName = "Root" } }.Concat(items);

            return items;
        }
    }
}
