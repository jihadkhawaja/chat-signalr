using System;
using System.ComponentModel.DataAnnotations;

namespace MobileChat.Web.Models
{
    public class UserFriend
    {
        [Key]
        public ulong Id { get; set; }
        [Key]
        public ulong UserId { get; set; }
        [Required]
        public string Username { get; set; }
        public string Email { get; set; }
        [Required]
        public string FriendUsername { get; set; }
        public string FriendEmail { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
