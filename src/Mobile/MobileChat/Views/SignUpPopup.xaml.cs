using MobileChat.ViewModel;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPopup : ContentView
    {
        private FriendsViewModel viewModel { get; set; }
        public SignUpPopup()
        {
            InitializeComponent();
        }

        public void Init(FriendsViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        private async void Create_Clicked(object sender, EventArgs e)
        {
            await viewModel.SignUp(displayName.Text, username.Text, null, password.Text);
        }

        private async void SignIn_Clicked(object sender, EventArgs e)
        {
            await viewModel.DisplaySignIn();
        }
    }
}