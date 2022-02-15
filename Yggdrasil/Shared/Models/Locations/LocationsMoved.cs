using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yggdrasil.Models.Locations
{
    /// <summary>
    /// Contains information about locations that have been relocated
    /// </summary>
    public sealed class LocationsMoved
    {
        /// <summary>
        /// Gets or sets a dictionary of locations as keys, and new parents as values
        /// </summary>
        public Dictionary<string, string> Locations { get; set; }
    }
}
