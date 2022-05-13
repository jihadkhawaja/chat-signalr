using MobileChat.Models;
using MobileChat.ViewModel;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class FriendsPage : ContentPage
    {
        private FriendsViewModel viewModel { get; set; }
        public FriendsPage()
        {
            BindingContext = viewModel = new FriendsViewModel();
            InitializeComponent();
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Channel channel = e.Item as Channel;

            Navigation.PushAsync(new ChatPage(viewModel.signalRService, viewModel.chatService, channel));
        }

        private async void AddUserButton(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Friend Username", "Enter your friend username");

            if (result != null)
            {
                await viewModel.chatService.AddFriend(viewModel.User.Id, result);

                await viewModel.chatService.CreateChannel(viewModel.User.Id, result);
            }
        }
    }
}