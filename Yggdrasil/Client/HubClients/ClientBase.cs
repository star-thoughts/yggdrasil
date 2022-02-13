using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Yggdrasil.Client.HubClients
{
    /// <summary>
    /// Base class for SignalR clients
    /// </summary>
    public abstract class ClientBase : IAsyncDisposable
    {
        public ClientBase(Uri uri, ILoggerProvider loggingProvider, Func<Task<string>> jwtProvider)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            _connection = new HubConnectionBuilder()
                .WithUrl(uri, options =>
                {
                    options.AccessTokenProvider = jwtProvider;
                })
                .WithAutomaticReconnect()
                .ConfigureLogging(logging =>
                {
                    logging.AddProvider(loggingProvider);
                    logging.SetMinimumLevel(LogLevel.Debug);
                })
                .Build();

            _connection.Reconnecting += _connection_Reconnecting;
            _connection.Reconnected += _connection_Reconnected;
            _connection.Closed += _connection_Closed;
        }

        private readonly HubConnection _connection;

        /// <summary>
        /// Gets whether or not this is connected to a server
        /// </summary>
        public bool IsConnected { get { return _connection.State == HubConnectionState.Connected; } }

        protected void AddMessageHandler<TEventArgs, TArg1>(string messageName, Action<TEventArgs> action) where TEventArgs : BaseEventArgs
        {
            _connection.On<string, TArg1>(messageName, (user, p1) =>
            {
                TEventArgs args = (TEventArgs)Activator.CreateInstance(typeof(TEventArgs), user, p1);
                action(args);
            });
        }

        protected void AddMessageHandler<TEventArgs, TArg1, TArg2>(string messageName, Action<TEventArgs> action) where TEventArgs : BaseEventArgs
        {
            _connection.On<string, TArg1, TArg2>(messageName, (user, p1, p2) =>
            {
                TEventArgs args = (TEventArgs)Activator.CreateInstance(typeof(TEventArgs), user, p1, p2);
                action(args);
            });
        }

        protected void AddMessageHandler<TEventArgs, TArg1, TArg2, TArg3>(string messageName, Action<TEventArgs> action) where TEventArgs : BaseEventArgs
        {
            _connection.On<string, TArg1, TArg2, TArg3>(messageName, (user, p1, p2, p3) =>
            {
                TEventArgs args = (TEventArgs)Activator.CreateInstance(typeof(TEventArgs), user, p1, p2, p3);
                action(args);
            });
        }

        public async Task Restart(CancellationToken cancellationToken = default)
        {
            TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();
            _connection.Closed += closed;

            await StopClient(cancellationToken);
            await completion.Task;
            await StartClient(cancellationToken);
            
            Task closed(Exception exc)
            {
                if (exc == null)
                    completion.TrySetResult(true);
                else
                    completion.TrySetException(exc);
                return Task.CompletedTask;
            };
        }

        public Task StartClient(CancellationToken cancellationToken = default)
        {
            return _connection.StartAsync(cancellationToken);
        }

        public Task StopClient(CancellationToken cancellationToken = default)
        {
            if (_connection.State == HubConnectionState.Connected)
                return _connection.StopAsync(cancellationToken);
            return Task.CompletedTask;
        }


        private Task _connection_Reconnecting(Exception arg)
        {
            Reconnecting?.Invoke(this, new EventArgs());
            return Task.CompletedTask;
        }

        private Task _connection_Reconnected(string arg)
        {
            Reconnected?.Invoke(this, new EventArgs());
            return Task.CompletedTask;
        }

        private Task _connection_Closed(Exception arg)
        {
            Disconnected?.Invoke(this, new EventArgs());
            return Task.CompletedTask;
        }
        /// <summary>
        /// Event that is triggerd when the server connection is lost and we are attepmting to reconnect
        /// </summary>
        public event EventHandler Reconnecting;
        /// <summary>
        /// Event that is triggered when we have lost a server connection, but reconnected
        /// </summary>
        public event EventHandler Reconnected;
        /// <summary>
        /// Event that is triggered when we have closed the connection
        /// </summary>
        public event EventHandler Disconnected;

        /// <summary>
        /// Disposes of this object
        /// </summary>
        /// <returns></returns>
        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return _connection.DisposeAsync();
        }
    }
}
