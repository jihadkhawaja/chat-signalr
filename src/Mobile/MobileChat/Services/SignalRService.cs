using Microsoft.AspNetCore.SignalR.Client;
using MobileChat.Interface;
using System;
using System.Threading.Tasks;

namespace MobileChat.Services
{
    public class SignalRService : ISignalR
    {
        public HubConnection HubConnection { get; set; }

        public event Func<Exception, Task> Reconnecting;
        public event Func<string, Task> Reconnected;
        public event Func<Exception, Task> Closed;
        public bool Initialize(string url)
        {
            try
            {
                HubConnection = new HubConnectionBuilder()
                .WithAutomaticReconnect(new TimeSpan[5]
                {
                    new TimeSpan(0,0,0),
                    new TimeSpan(0,0,5),
                    new TimeSpan(0,0,10),
                    new TimeSpan(0,0,30),
                    new TimeSpan(0,0,60)
                })
                .WithUrl(url)
                .Build();

                SubscribeHubEvents();

                return true;
            }
            catch { }

            return false;
        }
        private void SubscribeHubEvents()
        {
            HubConnection.Reconnected += HubConnection_Reconnected;
            HubConnection.Reconnecting += HubConnection_Reconnecting;
            HubConnection.Closed += HubConnection_Closed;
        }

        private Task HubConnection_Reconnecting(Exception arg)
        {
            Reconnecting?.Invoke(arg);

            return Task.CompletedTask;
        }

        private Task HubConnection_Reconnected(string arg)
        {
            Reconnected?.Invoke(arg);

            return Task.CompletedTask;
        }

        private Task HubConnection_Closed(Exception arg)
        {
            Closed?.Invoke(arg);

            return Task.CompletedTask;
        }

        public async Task<bool> Connect()
        {
            try
            {
                await HubConnection.StartAsync();

                return true;
            }
            catch { }

            return false;
        }
        public async Task<bool> Disconnect()
        {
            try
            {
                await HubConnection.StopAsync();

                return true;
            }
            catch { }

            return false;
        }
    }
}
