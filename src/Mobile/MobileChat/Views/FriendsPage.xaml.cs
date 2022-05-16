using MobileChat.Models;
using MobileChat.ViewModel;
using System;
using System.Threading.Tasks;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.signalRService.HubConnection == null)
                viewModel.Initialize();
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!viewModel.IsConnected) return;

            Channel channel = e.Item as Channel;

            Navigation.PushAsync(new ChatPage(viewModel.signalRService, viewModel.chatService, channel));
        }

        private async void AddUserButton(object sender, EventArgs e)
        {
            if (!viewModel.IsConnected) return;

            string result = await DisplayPromptAsync("Friend Username", "Enter your friend username");

            if (result != null)
            {
                await viewModel.chatService.AddFriend(viewModel.User.Id, result);

                await viewModel.chatService.CreateChannel(viewModel.User.Id, result);
            }
        }
    }
}