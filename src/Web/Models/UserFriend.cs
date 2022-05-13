﻿using System;
using System.ComponentModel.DataAnnotations;

namespace MobileChat.Web.Models
{
    public class UserFriend
    {
        [Key]
        public ulong Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid FriendUserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
