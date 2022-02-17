using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yggdrasil.Models.Locations
{
    /// <summary>
    /// Used for communicating a list of locations
    /// </summary>
    public sealed class LocationsList
    {
        /// <summary>
        /// Gets or sets a collection of locations
        /// </summary>
        public LocationListItem[] Locations { get; set; }
    }
}
