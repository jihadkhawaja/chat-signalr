using MobileChat.Interface;
using MobileChat.Models.CachedData;
using MobileChat.Models.Data;
using MobileChat.Models.ViewData;
using MobileChat.Themes;
using MobileChat.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class ChatPage : ContentPage
    {
        private ChatViewModel viewModel { get; set; }
        public ChatPage(ISignalR signalRService, IChat chatService, Channel channel)
        {
            viewModel = new ChatViewModel(signalRService, chatService, channel);
            
            BindingContext = viewModel;
            InitializeComponent();

            Subscribe();
        }

        protected override void OnAppearing()
        {
            App.CurrentPage = GetType().Name;

            //set theme
            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;

            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();
                switch (App.appSettings.theme)
                {
                    case AppSettings.Theme.Dark:
                        mergedDictionaries.Add(new Dark());
                        break;

                    case AppSettings.Theme.Light:
                        mergedDictionaries.Add(new Light());
                        break;

                    default:
                        mergedDictionaries.Add(new Dark());
                        break;
                }
            }

            ScrollToEnd(false);
        }
        protected override void OnDisappearing()
        {
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<ChatViewModel>(this, "ScrollToEnd", (sender) =>
            {
                ScrollToEnd();
            });
        }

        private void ScrollToEnd(bool animated = true)
        {
            object v = ChatList.ItemsSource.Cast<object>().LastOrDefault();
            ChatList.ScrollTo(v, ScrollToPosition.End, animated);
        }

        private void ChatList_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            //if ((ChatMessage)e.Item == viewModel.Messages[0])
            //{
            //    //First Item has been hit
            //}

            if ((ViewMessage)e.Item == viewModel.Messages[viewModel.Messages.Count - 1])
            {
                //Last Item has been hit
                viewModel.AutoScrollDownEnabled = true;
            }
            else
            {
                viewModel.AutoScrollDownEnabled = false;
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}