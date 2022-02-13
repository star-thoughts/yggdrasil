using Microsoft.AspNetCore.Components;
using System;

namespace Yggdrasil.Client.Pages.Exceptions
{
    public sealed partial class GenericExceptionView
    {
        /// <summary>
        /// Gets or sets hte exception to display
        /// </summary>
        [Parameter]
        public Exception Exception { get; set; }
        /// <summary>
        /// Gets or sets additional class features
        /// </summary>
        [Parameter]
        public string Class { get; set; }
        string GetClass()
        {
            return $"exception-details {Class}";
        }
    }
}
