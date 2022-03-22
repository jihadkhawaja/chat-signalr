using MobileChat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

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
            friends = new ObservableCollection<User> 
            { 
                new User { DisplayName = "Jhon Doe" },
                new User { DisplayName = "Jhon Doe" },
                new User { DisplayName = "Jhon Doe" },
                new User { DisplayName = "Jhon Doe" },
            };
        }
    }
}
