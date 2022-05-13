﻿using System;
using System.ComponentModel.DataAnnotations;

namespace MobileChat.Models
{
    public class Channel
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
