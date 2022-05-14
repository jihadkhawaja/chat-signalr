using Microsoft.AspNetCore.SignalR.Client;
using MobileChat.Interface;
using MobileChat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileChat.ViewModel
{
    public class ChatViewModel : ViewModelBase
    {
        public ISignalR signalRService { get; private set; }
        public IChat chatService { get; private set; }
        private Channel currentChannel;
        public Channel CurrentChannel
        {
            get => currentChannel;
            set
            {
                currentChannel = value;
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
        private string inputText;
        public string InputText
        {
            get => inputText;
            set
            {
                inputText = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Message> messages;
        public ObservableCollection<Message> Messages
        {
            get => messages;
            set
            {
                messages = value;
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

        public bool AutoScrollDown;

        public Command SendMessageCommand { get; }
        public Command ConnectCommand { get; }
        public Command DisconnectCommand { get; }

        public ChatViewModel(ISignalR signalRService, IChat chatService, Channel channel)
        {
            this.signalRService = signalRService;
            this.chatService = chatService;

            signalRService.Reconnected += Reconnected;
            signalRService.Reconnecting += Reconnecting;
            signalRService.Closed += Closed;

            Messages = new ObservableCollection<Message>();
            SendMessageCommand = new Command(async () => { await SendMessage(InputText, CurrentChannel.Id); });

            //set cached user credentials
            User = App.appSettings.user;
            CurrentChannel = channel;

            HubEvents();

            LoadChannelMessages();
        }

        private void HubEvents()
        {
            signalRService.HubConnection.On<Message>("ReceiveMessage", message =>
            {
                if (message.SenderId == User.Id)
                {
                    Message msg = Messages.Single(x => x.Id == message.Id);
                    //update messages
                    msg = message;
                }
                else
                {
                    message.Sent = false;
                    message.Seen = false;
                    Messages.Add(message);

                    if (AutoScrollDown)
                    {
                        MessagingCenter.Send(this, "ScrollToEnd");
                    }
                }
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
        
        private async Task SendMessage(string message, Guid channelId)
        {
            IsLoading = true;
            try
            {
                //clear input text box
                InputText = string.Empty;

                Message msg = new Message
                {
                    Id = Guid.NewGuid(),
                    ChannelId = channelId,
                    SenderId = User.Id,
                    Content = message,
                    DateCreated = DateTime.UtcNow,
                    Sent = false,
                    Seen = false,
                    IsYourMessage = true
                };
                Messages.Add(msg);

                MessagingCenter.Send(this, "ScrollToEnd");
                
                if (await chatService.SendMessage(msg))
                {
                    //message sent successfully
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            IsLoading = false;
        }

        private async Task LoadChannelMessages()
        {
            IsLoading = true;

            try
            {
                Message[] messages = await chatService.ReceiveMessageHistory(CurrentChannel.Id);

                foreach (Message msg in messages)
                {
                    if (msg.SenderId == User.Id)
                    {
                        msg.IsYourMessage = true;
                    }
                    else
                    {
                        msg.Sent = false;
                        msg.Seen = false;
                    }
                    
                    Messages.Add(msg);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            IsLoading = false;
        }
        
    }
}