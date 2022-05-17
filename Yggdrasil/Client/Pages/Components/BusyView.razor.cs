using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Yggdrasil.Client.Pages.Components
{
    /// <summary>
    /// View for showing busy indicator
    /// </summary>
    public partial class BusyView
    {
        /// <summary>
        /// Gets whether or not to show the busy indicator
        /// </summary>
        int BusyCount { get; set; }

        /// <summary>
        /// Starts a busy operation
        /// </summary>
        /// <returns>Task for asynchronous completion, dispose to complete the operation</returns>
        public async Task<IAsyncDisposable> BeginOperation()
        {
            BusyViewStateManager state = new BusyViewStateManager(this);
            await state.SetBusy();
            return state;
        }

        class BusyViewStateManager : IAsyncDisposable
        {
            public BusyViewStateManager(BusyView view)
            {
                _view = view;
            }

            BusyView _view;

            /// <summary>
            /// Sets the fact that a long-running operation has begun
            /// </summary>
            /// <returns>Task for asynchronous completion</returns>
            public async Task SetBusy()
            {
                _view.BusyCount++;
                await _view.InvokeAsync(_view.StateHasChanged);
            }

            /// <summary>
            /// Clears the busy indicator
            /// </summary>
            /// <returns>Task for asynchronous completion</returns>
            public async ValueTask DisposeAsync()
            {
                _view.BusyCount--;
                await _view.InvokeAsync(_view.StateHasChanged);
            }
        }
    }
}
