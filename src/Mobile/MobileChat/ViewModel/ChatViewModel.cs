using MobileChat.Cache;
using MobileChat.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using MobileChat.Views;

namespace MobileChat.ViewModel
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

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
        private string _totalusers;
        public string TotalUsers
        {
            get
            {
                return _totalusers;
            }
            set
            {
                _totalusers = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Message> _messages;
        public ObservableCollection<Message> Messages
        {
            get
            {
                return _messages;
            }
            set
            {
                _messages = value;
                OnPropertyChanged();
            }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }
        private bool _isConnected;
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            set
            {
                _isConnected = value;
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


        private HubConnection hubConnection;

        public Command SendMessageCommand { get; }
        public Command ConnectCommand { get; }
        public Command DisconnectCommand { get; }

        public ChatViewModel()
        {
            try
            {
                Messages = new ObservableCollection<Message>();
                SendMessageCommand = new Command(async () => { await SendMessage(UserMessage); });
                ConnectCommand = new Command(async () => await Connect());
                DisconnectCommand = new Command(async () => await Disconnect());

                User = App.appSettings.user;

                IsConnected = false;

                hubConnection = new HubConnectionBuilder()
                 .WithUrl(App.hubConnectionURL)
                 .Build();

                hubConnection.On<User>("JoinChat", o =>
                {
                    //todo
                    TotalUsers = $"[number] users in chat";
                });

                hubConnection.On<User>("LeaveChat", o =>
                {
                    //todo
                    TotalUsers = $"[number] users in chat";
                });

                hubConnection.On<User>("ReceiveAccount", async o =>
                {
                    User = App.appSettings.user = o;
                    SavingManager.JsonSerialization.WriteToJsonFile<AppSettings>("appsettings/user", App.appSettings);

                    await hubConnection.InvokeAsync("JoinChat", User);
                    await hubConnection.InvokeAsync("ReceiveMessageHistory", User);
                });

                hubConnection.On<Message>("ReceiveMessage", o =>
                {
                    if (o.UserId == User.Id)
                        o.IsYourMessage = true;
                    else
                        o.IsYourMessage = false;

                    Messages.Add(o);

                    if (AutoScrollDown)
                        MessagingCenter.Send<ChatViewModel>(this, "ScrollToEnd");
                });

                hubConnection.On<List<Message>>("ReceiveMessageHistory", o =>
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

                Connect();
            }
            catch { }
        }

        public async Task Connect()
        {
            if (IsLoading)
                return;

            IsLoading = true;
            try
            {
                if (!IsConnected)
                {
                    await hubConnection.StartAsync();

                    if (User is null) await DisplaySignUp();
                    else await SignIn(User);

                    IsConnected = true;
                }
            }
            catch (Exception e)
            {
                IsConnected = false;
                await App.Current.MainPage.DisplayAlert("Error", $"Connection lost, Connect to the internet and try again, Message:{e.Message}", "Ok");
            }

            IsLoading = false;
        }
        public async Task Disconnect()
        {
            await hubConnection.InvokeAsync("LeaveChat", User);
            await hubConnection.StopAsync();

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
                    msg.Content = Message;
                    await hubConnection.InvokeAsync("SendMessage", msg);
                    Message = string.Empty;
                }

                MessagingCenter.Send<ChatViewModel>(this, "ScrollToEnd");
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                await Connect();
            }
            IsLoading = false;
        }

        public async Task SignUp(User user)
        {
            try
            {
                await hubConnection.InvokeAsync("SignUp", user);

                PopupView = null;
            }
            catch { }
        }

        public async Task SignIn(User user)
        {
            try
            {
                await hubConnection.InvokeAsync("SignIn", user);

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