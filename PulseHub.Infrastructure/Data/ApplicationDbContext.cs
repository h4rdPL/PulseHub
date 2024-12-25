using Microsoft.EntityFrameworkCore;
using PulseHub.Core;

namespace PulseHub.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<Subscriptions> Subscriptions { get; set; }
    }
}
