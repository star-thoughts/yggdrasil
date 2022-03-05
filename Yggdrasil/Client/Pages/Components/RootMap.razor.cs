using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Yggdrasil.Client.Pages.Components
{
    /// <summary>
    /// Map for displaying ancestors of an item up to its root
    /// </summary>
    public partial class RootMap
    {
        /// <summary>
        /// Gets or sets the parent elements for the item
        /// </summary>
        [Parameter]
        public IEnumerable<RootMapItem> Ancestors { get; set; }
        /// <summary>
        /// Gets or sets an event that is triggered when an ancestor is selected
        /// </summary>
        [Parameter]
        public EventCallback<string> AncestorSelected { get; set; }

        async void OnParentSelected(string parent)
        {
            await AncestorSelected.InvokeAsync(parent);
        }
    }

    /// <summary>
    /// Contains information for displaying and selecting ancestor items in a <see cref="RootMap"/>
    /// </summary>
    public class RootMapItem
    {
        /// <summary>
        /// Gets or sets the name to display for the ancestor
        /// </summary>
        public string AncestorName { get; set; }
        /// <summary>
        /// Gets or sets the ID of the item to send as a notification
        /// </summary>
        public string AncestorId { get; set; }
    }
}
