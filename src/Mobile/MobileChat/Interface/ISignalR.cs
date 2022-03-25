using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MobileChat.Interface
{
    public interface ISignalR
    {
        bool Initialize(string url);
        Task<bool> Connect();
        Task<bool> Disconnect();
        HubConnection HubConnection { get; }
        event Func<Exception, Task> Reconnecting;
        event Func<string, Task> Reconnected;
        event Func<Exception, Task> Closed;
    }
}
