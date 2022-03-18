using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MobileChat.Models
{
    public class User
    {
        public ulong Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string About { get; set; }
        public string AvatarUrl { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirebaseToken { get; set; }
        public int Permission { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
