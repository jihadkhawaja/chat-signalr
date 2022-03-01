using System;
using System.Collections.Generic;
using System.Text;

namespace MobileChat.Models
{
    public class User
    {
        public ulong Id { get; set; }
        public string DisplayName { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
