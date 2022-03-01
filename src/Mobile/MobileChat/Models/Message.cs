using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MobileChat.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public ulong UserId { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        [JsonIgnore]
        public bool IsYourMessage { get; set; }
    }
}
