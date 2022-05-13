﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MobileChat.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid SenderId { get; set; }
        [Required]
        public Guid ChannelId { get; set; }
        [Required]
        public string DisplayName { get; set; }
        public bool Sent { get; set; }
        public DateTime DateSent { get; set; }
        public bool Seen { get; set; }
        public DateTime DateSeen { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }

        [JsonIgnore]
        public bool IsYourMessage { get; set; }
    }
}
