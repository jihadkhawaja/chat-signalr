using Microsoft.EntityFrameworkCore;
using MobileChat.Web.Models;

namespace MobileChat.Web.Database
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
#if DEBUG
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
#else
            options.UseSqlServer(Configuration.GetConnectionString("ProductionConnection"));
#endif
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserFriend> UsersFriends { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChannelUser> ChannelUsers { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
