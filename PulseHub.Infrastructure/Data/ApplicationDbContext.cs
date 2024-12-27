using Microsoft.EntityFrameworkCore;
using PulseHub.Core.Models;

namespace PulseHub.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Subscriptions> Subscriptions { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
