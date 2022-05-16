using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MobileChat.Interface
{
    public interface ISignalR
    {
        bool Initialize(string url);
        Task<bool> Connect(CancellationTokenSource cts);
        Task<bool> Disconnect();
        HubConnection HubConnection { get; }
        event Func<Exception, Task> Reconnecting;
        event Func<string, Task> Reconnected;
        event Func<Exception, Task> Closed;
    }
}
