using Microsoft.AspNetCore.SignalR.Client;
using MobileChat.Cache;
using MobileChat.Interface;
using MobileChat.Models;
using MobileChat.Services;
using MobileChat.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileChat.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        public ISignalR signalRService { get; set; }
        private Message _usermessage = new Message();
        public Message UserMessage
        {
            get
            {
                return _usermessage;
            }
            set
            {
                _usermessage = value;
                OnPropertyChanged();
            }
        }
        private User _user;
        public User User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }
        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }
        private string totalusers;
        public string TotalUsers
        {
            get
            {
                return totalusers;
            }
            set
            {
                totalusers = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Message> messages;
        public ObservableCollection<Message> Messages
        {
            get
            {
                return messages;
            }
            set
            {
                messages = value;
                OnPropertyChanged();
            }
        }
        private bool isLoading;
        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                isLoading = value;
                OnPropertyChanged();
            }
        }
        private bool isConnected;
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
            set
            {
                isConnected = value;
                OnPropertyChanged();
            }
        }

        public bool AutoScrollDown;

        private SignUpPopup signUpPopup { get; set; }
        private SignInPopup signInPopup { get; set; }

        private View popupView;

        public View PopupView
        {
            get
            {
                return popupView;
            }
            set
            {
                popupView = value;
                OnPropertyChanged();
            }
        }

        public Command SendMessageCommand { get; }
        public Command ConnectCommand { get; }
        public Command DisconnectCommand { get; }

        public ChatViewModel()
        {
            Messages = new ObservableCollection<Message>();
            SendMessageCommand = new Command(async () => { await SendMessage(UserMessage); });
            ConnectCommand = new Command(async () => await Connect());
            DisconnectCommand = new Command(async () => await Disconnect());

            User = App.appSettings.user;

            signalRService = DependencyService.Get<ISignalR>();

            if(signalRService.Initialize(App.hubConnectionURL))
            {
                HubEvents();

                Connect();
            }
            else
            {
                IsLoading = true;
                //alert message invalid hub connection url
                Application.Current.MainPage.DisplayAlert("Invalid Hub Connection URL", "Please check your hub connection url", "OK");
            }
        }

        private void HubEvents()
        {
            signalRService.Reconnected += Reconnected;
            signalRService.Reconnecting += Reconnecting;
            signalRService.Closed += Closed;

            signalRService.HubConnection.On<User>("JoinGlobalChat", o =>
            {
                //todo
            });

            signalRService.HubConnection.On<User>("LeaveGlobalChat", o =>
            {
                //todo
            });

            signalRService.HubConnection.On<int>("GlobalChatInfo", o =>
            {
                TotalUsers = $"{o} users in chat";
            });

            signalRService.HubConnection.On<User>("ReceiveAccount", async o =>
            {
                User = App.appSettings.user = o;
                SavingManager.JsonSerialization.WriteToJsonFile<AppSettings>("appsettings/user", App.appSettings);

                await signalRService.HubConnection.InvokeAsync("JoinGlobalChat", User.DisplayName);
                await signalRService.HubConnection.InvokeAsync("GlobalChatInfo");
                await signalRService.HubConnection.InvokeAsync("ReceiveGlobalMessageHistory");
            });

            signalRService.HubConnection.On<Message>("ReceiveMessage", o =>
            {
                if (o.UserId == User.Id)
                {
                    o.IsYourMessage = true;
                    o.Sent = true;
                }
                else
                {
                    o.IsYourMessage = false;
                    o.Seen = true;
                }

                Messages.Add(o);

                if (AutoScrollDown)
                    MessagingCenter.Send<ChatViewModel>(this, "ScrollToEnd");
            });

            signalRService.HubConnection.On<List<Message>>("ReceiveGlobalMessageHistory", o =>
            {
                Messages.Clear();
                foreach (Message cm in o)
                {
                    if (cm.UserId == User.Id)
                        cm.IsYourMessage = true;
                    else
                        cm.IsYourMessage = false;

                    Messages.Add(cm);
                }

                MessagingCenter.Send<ChatViewModel>(this, "ScrollToEnd");
            });
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

        public async Task Connect()
        {
            if (IsLoading)
                return;

            switch(signalRService.HubConnection.State)
            {
                case HubConnectionState.Connected:
                    return;
                case HubConnectionState.Disconnected:
                    IsLoading = true;

                    if (await signalRService.Connect())
                    {
                        if (User is null) await DisplaySignUp();
                        else await SignIn(User);

                        IsConnected = true;
                    }
                    else
                    {
                        IsConnected = false;
                    }

                    IsLoading = false;
                    break;
                case HubConnectionState.Connecting:
                    IsLoading = true;
                    break;
                case HubConnectionState.Reconnecting:
                    IsLoading = true;
                    break;
                default:
                    IsLoading = true;
                    break;
            }
        }
        public async Task Disconnect()
        {
            await signalRService.HubConnection.InvokeAsync("LeaveGlobalChat", User.DisplayName);
            await signalRService.Disconnect();

            IsConnected = false;
        }
        private async Task SendMessage(Message msg)
        {
            IsLoading = true;
            try
            {
                if (!string.IsNullOrEmpty(Message) && !string.IsNullOrWhiteSpace(Message))
                {
                    msg.UserId = User.Id;
                    msg.DisplayName = User.DisplayName;
                    msg.Content = Message;
                    await signalRService.HubConnection.InvokeAsync("SendMessage", msg);
                    Message = string.Empty;
                }

                MessagingCenter.Send<ChatViewModel>(this, "ScrollToEnd");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                //await Connect();
            }
            IsLoading = false;
        }

        public async Task SignUp(User user)
        {
            try
            {
                await signalRService.HubConnection.InvokeAsync("SignUp", user);

                PopupView = null;
            }
            catch { }
        }

        public async Task SignIn(User user)
        {
            try
            {
                await signalRService.HubConnection.InvokeAsync("SignIn", user);

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
    }
}