using Microsoft.EntityFrameworkCore;

namespace OnlineShopping.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BillingAddress> BillingAddresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItems> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BillingAddress>(entity =>
            {
                entity.HasIndex(i => i.AddressId).IsUnique();
                entity.Property(i => i.AddressId).HasDefaultValueSql("bill_ + CAST(Id AS VARCHAR(5))");
            });
        }
    }
}
