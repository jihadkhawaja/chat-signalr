using System;
using System.ComponentModel.DataAnnotations;

namespace MobileChat.Web.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public ulong UserId { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
