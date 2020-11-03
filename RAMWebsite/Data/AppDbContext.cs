using Microsoft.EntityFrameworkCore;
using RAMWebsite.Models;

namespace RAMWebsite.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Task> Tasks { get; set; }

        public DbSet<Test> Tests { get; set; }
    }
}
