using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yggdrasil.Models.Locations
{
    /// <summary>
    /// Enumeration of how to handle children when a parent location is changed (mainly deleted or moved)
    /// </summary>
    public enum HandleChildren
    {
        /// <summary>
        /// Value is unset and is considered invalid
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Child locations should become root locations
        /// </summary>
        MoveToRoot = 1,
        /// <summary>
        /// Child locations should be placed into the parent location of the updated location
        /// </summary>
        MoveToParent = 2,
        /// <summary>
        /// Child locations should be deleted (not currently supported)
        /// </summary>
        Delete = 3,
    }
}
