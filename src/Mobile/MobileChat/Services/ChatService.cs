using Microsoft.AspNetCore.SignalR.Client;
using MobileChat.Interface;
using MobileChat.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileChat.Services
{
    public class ChatService : IChat
    {
        private ISignalR signalRService { get; set; }
        public ChatService()
        {
            signalRService = DependencyService.Get<ISignalR>();
        }
        public async Task<KeyValuePair<Guid, bool>> SignUp(string displayname, string username, string email, string password)
        {
            return await signalRService.HubConnection.InvokeAsync<KeyValuePair<Guid, bool>>("SignUp", displayname, username, email, password);
        }

        public async Task<KeyValuePair<Guid, bool>> SignIn(string emailorusername, string password)
        {
            return await signalRService.HubConnection.InvokeAsync<KeyValuePair<Guid, bool>>("SignIn", emailorusername, password);
        }

        public async Task<bool> SendMessage(Message message)
        {
            return await signalRService.HubConnection.InvokeAsync<bool>("SendMessage", message);
        }

        public async Task<bool> AddFriend(Guid userId, string friendEmailorusername)
        {
            return await signalRService.HubConnection.InvokeAsync<bool>("AddFriend", userId, friendEmailorusername);
        }

        public async Task<bool> RemoveFriend(Guid userId, string friendEmailorusername)
        {
            return await signalRService.HubConnection.InvokeAsync<bool>("RemoveFriend", userId, friendEmailorusername);
        }

        public async Task<Channel> CreateChannel(Guid userId, params string[] usernames)
        {
            return await signalRService.HubConnection.InvokeAsync<Channel>("CreateChannel", userId, usernames);
        }
        public async Task<User[]> GetChannelUsers(Guid channelid)
        {
            return await signalRService.HubConnection.InvokeAsync<User[]>("GetChannelUsers", channelid);
        }

        public Task<Channel[]> GetUserChannels(Guid userid)
        {
            return signalRService.HubConnection.InvokeAsync<Channel[]>("GetUserChannels", userid);
        }

        public async Task<Message[]> ReceiveMessageHistory(Guid channelid)
        {
            return await signalRService.HubConnection.InvokeAsync<Message[]>("ReceiveMessageHistory", channelid);
        }

        public async Task<Message[]> ReceiveMessageHistoryRange(Guid channelid, int index, int range)
        {
            return await signalRService.HubConnection.InvokeAsync<Message[]>("ReceiveMessageHistoryRange", channelid, index, range);
        }

        public Task<string> GetUserDisplayName(Guid userId)
        {
            return signalRService.HubConnection.InvokeAsync<string>("GetUserDisplayName", userId);
        }
    }
}
