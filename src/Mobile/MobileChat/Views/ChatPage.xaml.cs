﻿using MobileChat.Models;
using MobileChat.Themes;
using MobileChat.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        public ChatPage()
        {
            this.BindingContext = App.chat;
            InitializeComponent();

            Subscribe();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.CurrentPage = this.GetType().Name;

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

            //TODO
            //if (string.IsNullOrEmpty(App.chat.chatmessage.UserName))
            //    await App.chat.CreateUsername();

            await App.chat.Connect();

            ScrollToEnd(false);
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
            var v = ChatList.ItemsSource.Cast<object>().LastOrDefault();
            ChatList.ScrollTo(v, ScrollToPosition.End, animated);
        }

        /// <summary>
        /// close fullscreen image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clicked(object sender, EventArgs e)
        {
            imageShowcaseHolder.IsVisible = false;
        }

        private void ChatList_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            //if ((ChatMessage)e.Item == App.chat.Messages[0])
            //{
            //    //First Item has been hit
            //}

            if ((Message)e.Item == App.chat.Messages[App.chat.Messages.Count - 1])
            {
                //Last Item has been hit
                App.chat.AutoScrollDown = true;
            }
            else
            {
                App.chat.AutoScrollDown = false;
            }
        }

        /// <summary>
        /// change User Name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await App.chat.CreateUsername(true);
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}