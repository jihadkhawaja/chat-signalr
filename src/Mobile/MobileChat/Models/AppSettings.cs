using System;

namespace MobileChat.Models
{
    public class AppSettings
    {
        public enum Theme
        {
            Dark,
            Light
        }

        public Theme theme { get; set; }
        public User user { get; set; }
    }
}