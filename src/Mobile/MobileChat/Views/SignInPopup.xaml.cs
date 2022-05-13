using MobileChat.ViewModel;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignInPopup : ContentView
    {
        private FriendsViewModel viewModel { get; set; }
        public SignInPopup()
        {
            InitializeComponent();
        }

        public void Init(FriendsViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        private void Create_Clicked(object sender, EventArgs e)
        {
            viewModel.DisplaySignUp();
        }

        private async void SignIn_Clicked(object sender, EventArgs e)
        {
            await viewModel.SignIn(username.Text, password.Text);
        }
    }
}