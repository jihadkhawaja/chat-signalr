using Microsoft.AspNetCore.SignalR;
using MobileChat.Web.Models;
using System;
using System.Threading.Tasks;

namespace MobileChatWeb.Hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
        public async Task JoinChat()
        {
            await ReceiveOldMessage();
            await Clients.All.SendAsync("JoinChat");
        }

        public async Task LeaveChat()
        {
            await Clients.All.SendAsync("LeaveChat");
        }

        public async Task SendMessage(Message msg)
        {
            //send msg to all suers in the global chat room
            await Clients.All.SendAsync("ReceiveMessage", msg);

            //save msg to db
        }
        public async Task ReceiveOldMessage()
        {
            //TODO
        }
    }
}