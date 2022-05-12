using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using MobileChat.Web.Interfaces;
using MobileChat.Web.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MobileChat.Web.Hubs
{
    public class ChatHub : Hub
    {
        private static List<string> GlobalChatConnections = new();
        public ChatHub(IUser userService, IMessage messageService)
        {
            this.userService = userService;
            this.messageService = messageService;
        }

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
            GlobalChatConnections.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
        public async Task SignUp(User user)
        {
            if(!await userService.UserExist(user.Username))
                await userService.Create(user);

            User registeredUser = await userService.ReadByUsername(user.Username);
            await Clients.Caller.SendAsync("ReceiveAccount", registeredUser);
        }
        public async Task SignIn(User user)
        {
            if (!await userService.UserExist(user.Username)) return;

            User registeredUser = await userService.ReadByUsername(user.Username);
            await Clients.Caller.SendAsync("ReceiveAccount", registeredUser);
        }
        public async Task ChangePassword(User user, string newpassword)
        {
            if (await userService.UserExist(user.Username))
            {
                user.Password = newpassword;
                await userService.Update(user);
            }   
        }
        public async Task JoinGlobalChat(string displayName)
        {
            await Clients.All.SendAsync("JoinGlobalChat", displayName);

            Debug.WriteLine("Connection Id: " + Context.ConnectionId);
            if(!GlobalChatConnections.Contains(Context.ConnectionId))
                GlobalChatConnections.Add(Context.ConnectionId);
        }

        public async Task LeaveGlobalChat(string displayName)
        {
            await Clients.All.SendAsync("LeaveGlobalChat", displayName);

            GlobalChatConnections.Remove(Context.ConnectionId);
        }

        public async Task GlobalChatInfo()
        {
            await Clients.All.SendAsync("GlobalChatInfo", GlobalChatConnections.Count);
        }

        public async Task SendMessage(Message msg)
        {
            msg.DateCreated = DateTime.UtcNow;

            //send msg to all suers in the global chat room
            await Clients.All.SendAsync("ReceiveMessage", msg);

            //save msg to db
            await messageService.Create(msg);
        }
        public async Task ReceiveGlobalMessageHistory()
        {
            HashSet<Message> msgs = await messageService.ReadAll();
            await Clients.Caller.SendAsync("ReceiveGlobalMessageHistory", msgs);
        }
        public async Task ReceiveMessageHistory(User user, User user2)
        {
            HashSet<Message> msgs = await messageService.ReadAll(user.Id, user2.Id);
            await Clients.Caller.SendAsync("ReceiveMessageHistory", msgs);
        }
        public async Task ReceiveMessageHistoryRange(User user, User user2, int index, int range)
        {
            HashSet<Message> msgs = (await messageService.ReadAll(user.Id, user2.Id)).Skip(index).Take(range).ToHashSet();
            await Clients.Caller.SendAsync("ReceiveMessageHistory", msgs);
        }
        public async Task AcceptFriend(User user, User user2)
        {
            await userService.AcceptFriendRequest(user, user2);

            await userService.AddFriend(user, user2);
        }
        public async Task RemoveFriend(User user, User user2)
        {
            await userService.RemoveFriend(user, user2);
        }
        public async Task GetFriends(User user)
        {
            await userService.GetUserFriends(user);
        }
        public async Task GetFriendRequests(User user)
        {
            await userService.GetUserFriendRequests(user);
        }
        public async Task BlockUser(User user, User user2)
        {
            await userService.BlockFriend(user, user2);
        }
        public async Task UnblockUser(User user, User user2)
        {
            await userService.UnblockFriend(user, user2);
        }
        public async Task RejectFriendRequest(User user, User user2)
        {
            await userService.RejectFriendRequest(user, user2);
        }
    }
}