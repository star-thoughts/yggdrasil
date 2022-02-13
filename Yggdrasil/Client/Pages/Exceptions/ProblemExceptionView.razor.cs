using Microsoft.AspNetCore.Components;
using Yggdrasil.Client.Services;

namespace Yggdrasil.Client.Pages.Exceptions
{
    public sealed partial class ProblemExceptionView
    {
        /// <summary>
        /// Gets or sets the exception to display
        /// </summary>
        [Parameter]
        public ProblemException Exception { get; set; }
        /// <summary>
        /// Gets or sets additional class states
        /// </summary>
        [Parameter]
        public string Class { get; set; }

        string GetClass()
        {
            return $"exception-details {Class}";
        }
    }
}
