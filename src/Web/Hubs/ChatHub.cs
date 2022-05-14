using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using MobileChat.Web.Helpers;
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
        public ChatHub(IUser userService, IMessage messageService, IChannel channelService)
        {
            this.userService = userService;
            this.messageService = messageService;
            this.channelService = channelService;
        }

        [Inject]
        private IUser userService { get; set; }
        [Inject]
        private IMessage messageService { get; set; }
        [Inject]
        private IChannel channelService { get; set; }
        public override Task OnConnectedAsync()
        {
            //set user IsOnline true when he connects or reconnects
            User connectedUser = userService.ReadByConnectionId(Context.ConnectionId).Result;
            if (connectedUser != null)
            {
                connectedUser.IsOnline = true;
                userService.Update(connectedUser);
            }

            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            //set user IsOnline false when he disconnects
            User connectedUser = userService.ReadByConnectionId(Context.ConnectionId).Result;
            if (connectedUser != null)
            {
                connectedUser.IsOnline = false;
                userService.Update(connectedUser);
            }

            return base.OnDisconnectedAsync(exception);
        }
        public async Task<KeyValuePair<Guid, bool>> SignUp(string displayname, string username, string email, string password)
        {
            if (await userService.UserExist(username))
            {
                return new KeyValuePair<Guid, bool>(Guid.Empty, false);
            }

            User user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                Password = password,
                DisplayName = displayname,
                ConnectionId = Context.ConnectionId,
                DateCreated = DateTime.UtcNow,
                IsOnline = true
            };

            if (await userService.Create(user))
            {
                return new KeyValuePair<Guid, bool>(user.Id, true);
            }

            return new KeyValuePair<Guid, bool>(Guid.Empty, false);
        }
        public async Task<KeyValuePair<Guid, bool>> SignIn(string emailorusername, string password)
        {
            if (!await userService.UserExist(emailorusername))
            {
                return new KeyValuePair<Guid, bool>(Guid.Empty, false);
            }

            if (!await userService.SignIn(emailorusername, password))
            {
                return new KeyValuePair<Guid, bool>(Guid.Empty, false);
            }

            if (PatternMatchHelper.IsEmail(emailorusername))
            {
                User registeredUser = await userService.ReadByEmail(emailorusername);
                registeredUser.ConnectionId = Context.ConnectionId;
                registeredUser.IsOnline = true;
                await userService.Update(registeredUser);

                return new KeyValuePair<Guid, bool>(registeredUser.Id, true);
            }
            else
            {
                User registeredUser = await userService.ReadByUsername(emailorusername);
                registeredUser.ConnectionId = Context.ConnectionId;
                registeredUser.IsOnline = true;
                await userService.Update(registeredUser);
                
                return new KeyValuePair<Guid, bool>(registeredUser.Id, true);                
            }
        }
        public async Task<bool> ChangePassword(string emailorusername, string newpassword)
        {
            if (await userService.UserExist(emailorusername))
            {
                if (PatternMatchHelper.IsEmail(emailorusername))
                {
                    User registeredUser = await userService.ReadByEmail(emailorusername);
                    registeredUser.Password = newpassword;
                    await userService.Update(registeredUser);

                    return true;
                }
                else
                {
                    User registeredUser = await userService.ReadByUsername(emailorusername);
                    registeredUser.Password = newpassword;
                    await userService.Update(registeredUser);
                    
                    return true;
                }
            }

            return false;
        }
        public async Task<string> GetUserDisplayName(Guid userId)
        {
            string displayname = await userService.GetDisplayName(userId);
            return displayname;
        }

        public async Task<Channel> CreateChannel(Guid userId, params string[] usernames)
        {
            Channel channel = new Channel
            {
                Id = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
            };

            await channelService.Create(channel);
            await channelService.AddUsers(userId, channel.Id, usernames);

            return channel;
        }
        public async Task<User[]> GetChannelUsers(Guid channelid)
        {
            HashSet<User> channelUsers = await channelService.GetUsers(channelid);
            //only send users ids and display names
            List<User> users = new List<User>();
            foreach (User user in channelUsers)
            {
                users.Add(new User
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName
                });
            }
            return users.ToArray();
        }
        public async Task<Channel[]> GetUserChannels(Guid userid)
        {
            HashSet<Channel> userChannels = await channelService.GetUserChannels(userid);
            return userChannels.ToArray();
        }

        public async Task<bool> SendMessage(Message message)
        {
            if (message == null)
            {
                return false;
            }

            if (message.SenderId == Guid.Empty)
            {
                return false;
            }

            if (string.IsNullOrEmpty(message.Content) || string.IsNullOrWhiteSpace(message.Content))
            {
                return false;
            }

            //save msg to db
            message.Sent = true;
            message.DateSent = DateTime.UtcNow;
            if (await messageService.Create(message))
            {
                foreach (User user in await channelService.GetUsers(message.ChannelId))
                {
                    await Clients.Client(user.ConnectionId).SendAsync("ReceiveMessage", message);
                }

                return true;
            }

            return false;
        }
        public async Task<bool> UpdateMessage(Message message)
        {
            if (message == null)
            {
                return false;
            }

            if (message.SenderId == Guid.Empty)
            {
                return false;
            }

            if (string.IsNullOrEmpty(message.Content) || string.IsNullOrWhiteSpace(message.Content))
            {
                return false;
            }

            //save msg to db
            if (await messageService.Update(message))
            {
                foreach (User user in await channelService.GetUsers(message.ChannelId))
                {
                    await Clients.Client(user.ConnectionId).SendAsync("ReceiveMessage", message);
                }

                return true;
            }

            return false;
        }
        public async Task<Message[]> ReceiveMessageHistory(Guid channelId)
        {
            HashSet<Message> msgs = await channelService.GetChannelMessages(channelId);
            return msgs.ToArray();
        }
        public async Task<Message[]> ReceiveMessageHistoryRange(Guid channelId, int index, int range)
        {
            HashSet<Message> msgs = (await channelService.GetChannelMessages(channelId)).Skip(index).Take(range).ToHashSet();
            return msgs.ToArray();
        }
        public async Task<bool> AddFriend(Guid userId, string friendEmailorusername)
        {
            if (await userService.AddFriend(userId, friendEmailorusername)) return true;
            else return false;
        }
        public async Task<bool> RemoveFriend(Guid userId, string friendEmailorusername)
        {
            if (await userService.RemoveFriend(userId, friendEmailorusername)) return true;
            else return false;
        }
    }
}