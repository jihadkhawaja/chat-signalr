using MobileChatWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace MobileChatWeb.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
