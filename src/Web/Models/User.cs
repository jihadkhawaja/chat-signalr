using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MobileChatWeb.Models
{
    public class User
    {
        public User() { }
        public User(string username, string email, string password)
        {
            Username = username;
            Email = email;
            Password = password;
        }

        [Key]
        public ulong Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string About { get; set; }
        public string AvatarUrl { get; set; }
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string FirebaseToken { get; set; }
        [Required]
        public int Permission { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
