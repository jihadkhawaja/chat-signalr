using MobileChat.Interface;
using MobileChat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileChat.ViewModel
{
    public class FriendsViewModel : ViewModelBase
    {
        public ISignalR signalRService { get; private set; }
        private ObservableCollection<User> friends;
        public ObservableCollection<User> Friends
        {
            get { return friends; }
            set { friends = value; }
        }

        public FriendsViewModel()
        {
            signalRService = DependencyService.Get<ISignalR>();

            LoadFriends();
        }

        public Task AddFriend(User user)
        {
            friends.Add(user);

            return Task.CompletedTask;
        }

        public Task RemoveFriend(User user)
        {
            friends.Remove(user);

            return Task.CompletedTask;
        }

        public Task LoadFriends()
        {
            //fake data
            Friends = new ObservableCollection<User> 
            { 
                new User 
                {
                    DisplayName = "Jhon Doe" 
                },
                new User
                {
                    DisplayName = "Jhon Doe 2"
                },
            };

            return Task.CompletedTask;
        }
    }
}
