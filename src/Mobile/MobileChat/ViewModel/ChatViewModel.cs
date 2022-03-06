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

        public int totalusers = 0;

        private Message _usermessage = new Message();
        public Message chatmessage
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
        private User _user = new User();
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

        private HubConnection hubConnection;

        public Command SendMessageCommand { get; }
        public Command ConnectCommand { get; }
        public Command DisconnectCommand { get; }

        public ChatViewModel()
        {
            try
            {
                Messages = new ObservableCollection<Message>();
                SendMessageCommand = new Command(async () => { await SendMessage(chatmessage); });
                ConnectCommand = new Command(async () => await Connect());
                DisconnectCommand = new Command(async () => await Disconnect());

                User = new User();

                IsConnected = false;

                hubConnection = new HubConnectionBuilder()
                 .WithUrl(App.hubConnectionURL)
                 .Build();

                hubConnection.On<User>("JoinChat", o =>
                {
                    totalusers++;
                    TotalUsers = $"{totalusers} users in chat";
                });

                hubConnection.On<User>("LeaveChat", o =>
                {
                    totalusers--;
                    TotalUsers = $"{totalusers} users in chat";
                });

                hubConnection.On<Message>("ReceiveMessage", chatmessage =>
                {
                    if (chatmessage.UserId == User.Id)
                        chatmessage.IsYourMessage = true;
                    else
                        chatmessage.IsYourMessage = false;

                    Messages.Add(chatmessage);

                    if (AutoScrollDown)
                        MessagingCenter.Send<ChatViewModel>(this, "ScrollToEnd");
                });

                hubConnection.On<List<Message>>("ReceiveOldMessage", chatmessages =>
                {
                    Messages.Clear();
                    foreach (Message cm in chatmessages)
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
                    await hubConnection.InvokeAsync("JoinChat");

                    IsConnected = true;
                }
            }
            catch (Exception e)
            {
                IsConnected = false;
                await App.Current.MainPage.DisplayAlert("Error", "Connection lost, Connect to the internet and try again", "Ok");
            }

            IsLoading = false;
        }
        public async Task Disconnect()
        {
            await hubConnection.InvokeAsync("LeaveChat");
            await hubConnection.StopAsync();

            IsConnected = false;
        }
        private async Task SendMessage(Message chatmessage)
        {
            IsLoading = true;
            try
            {
                if (!string.IsNullOrEmpty(Message) && !string.IsNullOrWhiteSpace(Message))
                {
                    chatmessage.Content = Message;
                    await hubConnection.InvokeAsync("SendMessage", chatmessage);
                    Message = string.Empty;
                }

                MessagingCenter.Send<ChatViewModel>(this, "ScrollToEnd");
            }
            catch
            {
                await Connect();
            }
            IsLoading = false;
        }

        public async Task CreateUsername(bool overwrite = false)
        {
            if (!string.IsNullOrEmpty(App.appSettings.chatUserName) && !overwrite)
            {
                User.DisplayName = "Logged as " + SavingManager.JsonSerialization.ReadFromJsonFile<AppSettings>("appsettings /user").chatUserName;
                return;
            }

            string results = await App.Current.MainPage.DisplayPromptAsync("What should we call you?", "Enter your display name", "Apply", "Close",
                "Anonymous", 50, Keyboard.Chat, "");

            if ((string.IsNullOrEmpty(results) || string.IsNullOrWhiteSpace(results)) && !string.IsNullOrEmpty(App.appSettings.chatUserName))
                return;
            else if (string.IsNullOrEmpty(results) || string.IsNullOrWhiteSpace(results))
                App.appSettings.chatUserName = "Anonymous";
            else
                App.appSettings.chatUserName = results;
            User.DisplayName = "Logged as " + App.appSettings.chatUserName;

            SavingManager.JsonSerialization.WriteToJsonFile<AppSettings>("appsettings/user", App.appSettings);
        }
    }
}