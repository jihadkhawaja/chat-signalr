using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using MobileChat.Web.Interfaces;
using MobileChat.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task CreateUser(User user)
        {
            if(!await userService.UserExist(user.Username, user.Email))
                await userService.Create(user);
        }
        public async Task ChangePassword(User user, string newpassword)
        {
            if (await userService.UserExist(user.Username, user.Email))
            {
                user.Password = newpassword;
                await userService.Update(user);
            }   
        }
        public async Task JoinChat(User user)
        {
            await Clients.All.SendAsync("JoinChat", user);
        }

        public async Task LeaveChat(User user)
        {
            await Clients.All.SendAsync("LeaveChat", user);
        }

        public async Task SendMessage(Message msg)
        {
            //send msg to all suers in the global chat room
            await Clients.All.SendAsync("ReceiveMessage", msg);

            //save msg to db
            await messageService.Create(msg);
        }
        public async Task ReceiveMessageHistory(User user)
        {
            HashSet<Message> msgs = await messageService.ReadAll(user.Id);
            await Clients.Caller.SendAsync("ReceiveMessageHistory", msgs);
        }
        public async Task ReceiveMessageHistory(User user, int index, int range)
        {
            HashSet<Message> msgs = (await messageService.ReadAll(user.Id)).Skip(index).Take(range).ToHashSet();
            await Clients.Caller.SendAsync("ReceiveMessageHistory", msgs);
        }
    }
}