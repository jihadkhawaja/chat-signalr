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
    public partial class FriendsPage : ContentPage
    {
        FriendsViewModel viewModel { get; set; } 
        public FriendsPage()
        {
            BindingContext = viewModel = new FriendsViewModel();
            InitializeComponent();
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            User user = e.Item as User;
        }
    }
}