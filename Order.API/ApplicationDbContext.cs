using Microsoft.EntityFrameworkCore;

namespace Order.API
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Shared.Order> Orders { get; set; }
        public DbSet<Shared.OrderItem> OrderItems { get; set; }
    }
}
