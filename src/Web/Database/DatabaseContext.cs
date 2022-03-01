using Microsoft.EntityFrameworkCore;
using MobileChatWeb.Models;

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
