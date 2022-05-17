﻿using Microsoft.AspNetCore.SignalR.Client;
using MobileChat.Cache;
using MobileChat.Interface;
using MobileChat.Models.Data;
using MobileChat.Models.ViewData;
using MobileChat.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileChat.ViewModel
{
    public class FriendsViewModel : ViewModelBase
    {
        public ISignalR signalRService { get; private set; }
        public IChat chatService { get; private set; }
        
        private ObservableCollection<ViewChannel> channels;
        public ObservableCollection<ViewChannel> Channels
        {
            get => channels;
            set
            {
                channels = value;
                OnPropertyChanged();
            }
        }
        
        private SignUpPopup signUpPopup { get; set; }
        private SignInPopup signInPopup { get; set; }

        private View popupView;
        public View PopupView
        {
            get => popupView;
            set
            {
                popupView = value;
                OnPropertyChanged();
            }
        }
        
        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                OnPropertyChanged();
            }
        }
        
        private bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                isConnected = value;
                OnPropertyChanged();
            }
        }
        
        private User user;
        public User User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
            }
        }

        public FriendsViewModel()
        {
            signalRService = DependencyService.Get<ISignalR>();
            chatService = DependencyService.Get<IChat>();

            signalRService.Reconnected += Reconnected;
            signalRService.Reconnecting += Reconnecting;
            signalRService.Closed += Closed;

            Channels = new ObservableCollection<ViewChannel>();

            //set cached user credentials
            User = App.appSettings.user;
        }

        private Task Reconnecting(Exception arg)
        {
            IsConnected = false;
            IsLoading = true;

            return Task.CompletedTask;
        }

        private Task Reconnected(string arg)
        {
            IsConnected = true;
            IsLoading = false;

            return Task.CompletedTask;
        }

        private Task Closed(Exception arg)
        {
            IsConnected = false;
            IsLoading = true;

            return Task.CompletedTask;
        }

        public async Task Disconnect()
        {
            await signalRService.Disconnect();

            IsConnected = false;
            IsLoading = true;            
        }

        public async void Initialize()
        {
            Channels = new ObservableCollection<ViewChannel>();
            IsLoading = true;

            if (signalRService.Initialize(App.hubConnectionURL))
            {
                await Connect();
            }
            else
            {
                //alert message invalid hub connection url
                await Application.Current.MainPage.DisplayAlert("Invalid Hub Connection URL", "Please check your hub connection url", "OK");
            }
        }

        public async Task AddFriend(Guid userId, string friendEmailorusername)
        {
            if(await chatService.AddFriend(userId, friendEmailorusername))
            {
                await chatService.CreateChannel(userId, friendEmailorusername);
            } 
        }

        public async Task RemoveFriend(Guid userId, string friendEmailorusername)
        {
            await chatService.RemoveFriend(userId, friendEmailorusername);

            User user = new User
            {
                Id = userId,
                Username = friendEmailorusername
            };
        }

        public async Task LoadFriends()
        {
            Channel[] channels = await chatService.GetUserChannels(User.Id);
            ViewChannel[] viewChannels = new ViewChannel[channels.Length];

            for (int i = 0; i < channels.Length; i++)
            {
                viewChannels[i] = new ViewChannel();
                viewChannels[i].Channel = channels[i];
                User[] friends = await chatService.GetChannelUsers(viewChannels[i].Channel.Id);
                foreach (User friend in friends)
                {
                    viewChannels[i].Name += ", ";

                    if (friend.Id == User.Id)
                    {
                        viewChannels[i].Name = viewChannels[i].Name.Insert(0, "You");
                    }
                    else
                    {
                        viewChannels[i].Name += await chatService.GetUserDisplayName(friend.Id);
                    }
                }

                viewChannels[i].Name = viewChannels[i].Name.TrimEnd(',', ' ');

                Channels.Add(viewChannels[i]);
            }
        }

        public async Task SignUp(string displayname, string username, string email, string password)
        {
            try
            {
                App.appSettings.user = new User(username, null, password);
                App.appSettings.user.DisplayName = displayname;

                KeyValuePair<Guid, bool> result = await chatService.SignUp(displayname, username, null, password);
                if (result.Value)
                {
                    App.appSettings.user.Id = result.Key;
                    SavingManager.JsonSerialization.WriteToJsonFile("appsettings/user", App.appSettings);
                    PopupView = null;
                }
                else
                {
                    App.appSettings.user = null;
                    await Application.Current.MainPage.DisplayAlert("Sign Up Failed", "Please check your credentials", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                App.appSettings.user = null;
                await Application.Current.MainPage.DisplayAlert("Sign Up Failed", "Please check your credentials", "OK");
            }
        }

        public async Task SignIn(string username, string password)
        {
            try
            {
                KeyValuePair<Guid, bool> result = await chatService.SignIn(username, password);
                if (result.Value)
                {
                    App.appSettings.user = new User(username, null, password);
                    App.appSettings.user.Id = result.Key;
                    App.appSettings.user.DisplayName = await chatService.GetUserDisplayName(result.Key);
                    SavingManager.JsonSerialization.WriteToJsonFile("appsettings/user", App.appSettings);

                    User = App.appSettings.user;
                    PopupView = null;
                }
                else
                {
                    App.appSettings.user = null;
                    await Application.Current.MainPage.DisplayAlert("Invalid Credentials", "Please check your credentials", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                App.appSettings.user = null;
                await Application.Current.MainPage.DisplayAlert("Sign In Failed", "Please check your credentials", "OK");
            }
        }

        public Task DisplaySignUp()
        {
            PopupView = signUpPopup = new SignUpPopup();
            signUpPopup.Init(this);

            return Task.CompletedTask;
        }

        public Task DisplaySignIn()
        {
            PopupView = signInPopup = new SignInPopup();
            signInPopup.Init(this);

            return Task.CompletedTask;
        }

        public async Task Connect()
        {
            if (IsConnected)
            {
                return;
            }
            
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(15));
            if (await signalRService.Connect(cts))
            {
                Debug.WriteLine("Connection ID: " + signalRService.HubConnection.ConnectionId);

                if (User is null)
                {
                    await DisplaySignUp();
                }
                else
                {
                    if (!(await chatService.SignIn(User.Username, User.Password)).Value)
                    {
                        await DisplaySignIn();
                    }
                }

                await LoadFriends();

                IsConnected = true;
                IsLoading = false;
            }
            else
            {
                IsConnected = false;
                IsLoading = true;
                //alert message invalid hub connection url
                await Application.Current.MainPage.DisplayAlert("Invalid Hub Connection URL", "Please check your hub connection url", "OK");
            }
        }
    }
}
