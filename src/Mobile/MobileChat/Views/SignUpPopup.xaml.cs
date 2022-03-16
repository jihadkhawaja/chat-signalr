using MobileChat.Models;
using MobileChat.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MobileChat.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPopup : ContentView
    {
        ChatViewModel viewModel { get; set; }
        public SignUpPopup()
        {
            InitializeComponent();
        }

        public void Init(ChatViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        private async void Create_Clicked(object sender, EventArgs e)
        {
            User user = new User { DisplayName = displayName.Text, Username = username.Text, Password = password.Text };
            await viewModel.SignUp(user);
        }

        private async void SignIn_Clicked(object sender, EventArgs e)
        {
            await viewModel.DisplaySignIn();
        }
    }
}