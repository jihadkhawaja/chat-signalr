using Microsoft.AspNetCore.SignalR.Client;
using MobileChat.Cache;
using MobileChat.Interface;
using MobileChat.Models;
using MobileChat.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileChat.ViewModel
{
    public class FriendsViewModel : ViewModelBase
    {
        public ISignalR signalRService { get; private set; }
        public IChat chatService { get; private set; }
        private ObservableCollection<Channel> channels;
        public ObservableCollection<Channel> Channels
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

            //set cached user credentials
            User = App.appSettings.user;

            if (signalRService.Initialize(App.hubConnectionURL))
            {
                Connect();
            }
            else
            {
                IsLoading = true;
                //alert message invalid hub connection url
                Application.Current.MainPage.DisplayAlert("Invalid Hub Connection URL", "Please check your hub connection url", "OK");
            }
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
            Channels = new ObservableCollection<Channel>(channels);
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
                    IsLoading = false;
                }
                else
                {
                    IsLoading = true;
                    App.appSettings.user = null;
                    await Application.Current.MainPage.DisplayAlert("Sign Up Failed", "Please check your credentials", "OK");
                    await DisplaySignUp();
                }

                PopupView = null;
            }
            catch { }
        }

        public async Task SignIn(string emailorusername, string password)
        {
            try
            {
                KeyValuePair<Guid, bool> result = await chatService.SignIn(emailorusername, password);
                if (result.Value)
                {
                    App.appSettings.user.Id = result.Key;
                    App.appSettings.user.Username = emailorusername;
                    App.appSettings.user.DisplayName = await chatService.GetUserDisplayName(result.Key);
                    SavingManager.JsonSerialization.WriteToJsonFile("appsettings/user", App.appSettings);

                    User = App.appSettings.user;
                    IsLoading = false;
                }
                else
                {
                    IsLoading = true;
                    App.appSettings.user = null;
                    await Application.Current.MainPage.DisplayAlert("Invalid Credentials", "Please check your credentials", "OK");
                    await DisplaySignIn();
                }

                PopupView = null;
            }
            catch { }
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
            if (IsLoading)
            {
                return;
            }

            switch (signalRService.HubConnection.State)
            {
                case HubConnectionState.Connected:
                    return;
                case HubConnectionState.Disconnected:
                    if (await signalRService.Connect())
                    {
                        Debug.WriteLine("Connection ID: " + signalRService.HubConnection.ConnectionId);

                        if (User is null)
                        {
                            await DisplaySignUp();
                        }
                        else
                        {
                            if(!(await chatService.SignIn(User.Username, User.Password)).Value)
                            {
                                await DisplaySignIn();
                            }
                        }

                        await LoadFriends();

                        IsConnected = true;
                    }
                    else
                    {
                        IsConnected = false;
                    }
                    break;
                case HubConnectionState.Connecting:
                    break;
                case HubConnectionState.Reconnecting:
                    break;
                default:
                    break;
            }
        }
    }
}
