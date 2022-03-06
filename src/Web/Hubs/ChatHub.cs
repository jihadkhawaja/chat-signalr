using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using MobileChat.Web.Interfaces;
using MobileChat.Web.Models;
using System;
using System.Threading.Tasks;

namespace MobileChat.Web.Hubs
{
    public class ChatHub : Hub
    {
        [Inject]
        private IUser userService { get; set; }
        [Inject]
        private IMessage messageService { get; set; }
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
            await ReceiveMessageHistory();
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
        public async Task ReceiveMessageHistory()
        {
            //TODO
        }
    }
}