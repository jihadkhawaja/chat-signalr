﻿using Microsoft.AspNetCore.SignalR.Client;
using MobileChat.Interface;
using MobileChat.Models.Data;
using MobileChat.Models.ViewData;
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

        public Command SendMessageCommand { get; }
        
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

        public bool AutoScrollDownEnabled { get; set; }

        private ObservableCollection<ViewMessage> messages;
        public ObservableCollection<ViewMessage> Messages
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
        

        public ChatViewModel(ISignalR signalRService, IChat chatService, Channel channel)
        {
            this.signalRService = signalRService;
            this.chatService = chatService;

            signalRService.Reconnected += Reconnected;
            signalRService.Reconnecting += Reconnecting;
            signalRService.Closed += Closed;

            SendMessageCommand = new Command(async () => { await SendMessage(InputText, CurrentChannel.Id); });

            Messages = new ObservableCollection<ViewMessage>();

            //set cached user credentials
            User = App.appSettings.user;
            CurrentChannel = channel;

            HubEvents();
            
            Task.Factory.StartNew(LoadChannelMessages);
        }

        private void HubEvents()
        {
            signalRService.HubConnection.On<Message>("ReceiveMessage", message =>
            {
                if (message.SenderId == User.Id)
                {
                    ViewMessage viewMessage = Messages.Single(x => x.Message.Id == message.Id);
                    //update messages
                    viewMessage.IsYourMessage = true;
                    viewMessage.Message = message;
                }
                else
                {
                    message.Sent = false;
                    message.Seen = false;

                    ViewMessage viewMessage = new ViewMessage()
                    {
                        Message = message,
                        IsYourMessage = false
                    };

                    Messages.Add(viewMessage);

                    if (AutoScrollDownEnabled)
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
                };
                ViewMessage viewMessage = new ViewMessage
                {
                    Message = msg,
                    IsYourMessage = true,
                };
                Messages.Add(viewMessage);

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
                ViewMessage[] viewMessages = new ViewMessage[messages.Length];
                
                for (int i = 0; i < messages.Length; i++)
                {
                    viewMessages[i] = new ViewMessage();
                    viewMessages[i].Message = messages[i];
                    if (viewMessages[i].Message.SenderId == User.Id)
                    {
                        viewMessages[i].Message.DisplayName = User.DisplayName;
                        viewMessages[i].IsYourMessage = true;
                    }
                    else
                    {
                        viewMessages[i].Message.DisplayName = await chatService.GetUserDisplayName(messages[i].SenderId);
                        viewMessages[i].Message.Sent = false;
                        viewMessages[i].Message.Seen = false;
                    }
                    
                    Messages.Add(viewMessages[i]);
                }

                MessagingCenter.Send(this, "ScrollToEnd");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            IsLoading = false;
        }
        
    }
}