using MobileChat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace MobileChat.ViewModel
{
    public class FriendsViewModel : ViewModelBase
    {
        private ObservableCollection<User> friends;

        public ObservableCollection<User> Friends
        {
            get { return friends; }
            set { friends = value; }
        }

        public FriendsViewModel()
        {
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
            return Task.CompletedTask;
        }
    }
}
