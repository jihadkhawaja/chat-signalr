using MobileChat.Models;
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
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class ChatPage : ContentPage
    {
        private ChatViewModel viewModel { get; set; }
        public ChatPage()
        {
            BindingContext = viewModel = new ChatViewModel();
            InitializeComponent();

            Subscribe();
        }

        protected override void OnAppearing()
        {
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

            ScrollToEnd(false);
        }
        protected override void OnDisappearing()
        {
            viewModel.Disconnect();
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
            //if ((ChatMessage)e.Item == viewModel.Messages[0])
            //{
            //    //First Item has been hit
            //}

            if ((Message)e.Item == viewModel.Messages[viewModel.Messages.Count - 1])
            {
                //Last Item has been hit
                viewModel.AutoScrollDown = true;
            }
            else
            {
                viewModel.AutoScrollDown = false;
            }
        }

        /// <summary>
        /// change User Name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //change username
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}