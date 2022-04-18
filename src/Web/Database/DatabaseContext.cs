using Microsoft.EntityFrameworkCore;
using MobileChat.Web.Models;

namespace MobileChat.Web.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserFriend> UsersFriends { get; set; }
    }
}
