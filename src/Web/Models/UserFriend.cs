using System;
using System.ComponentModel.DataAnnotations;

namespace MobileChat.Web.Models
{
    public class UserFriend
    {
        [Key]
        public ulong Id { get; set; }
        [Required]
        public ulong UserId { get; set; }
        [Required]
        public ulong FriendUserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
