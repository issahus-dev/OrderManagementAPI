using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Models;
namespace OrderManagementAPI.Data

{
    public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
    {
        //Table
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Customer> Customers => Set<Customer>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Orders
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderID);
                entity.HasOne(o => o.Customer).
                WithMany(c => c.Orders).
                HasForeignKey(o => o.CustomerID);
                // cannot be null and must not exceed max lenght
                // Adding validation at database level to prevent duplicates
                entity.Property(o => o.CustomerID).IsRequired();


                entity.HasIndex(o => new { o.CustomerID, o.OrderDate, o.OrderValue })
                 .IsUnique();
            });
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c =>c.CustomerID);
                entity.HasMany(c => c.Orders).
                WithOne(o => o.Customer).
                HasForeignKey(o => o.OrderID);
            });
            
        }

    }
}
