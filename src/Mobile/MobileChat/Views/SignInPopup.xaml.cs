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
    public partial class SignInPopup : ContentView
    {
        ChatViewModel viewModel { get; set; }
        public SignInPopup()
        {
            InitializeComponent();
        }

        public void Init(ChatViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        private void Create_Clicked(object sender, EventArgs e)
        {
            viewModel.DisplaySignUp();
        }

        private async void SignIn_Clicked(object sender, EventArgs e)
        {
            User user = new User { Username = username.Text, Password = password.Text };
            await viewModel.SignIn(user);
        }
    }
}