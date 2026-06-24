using Microsoft.EntityFrameworkCore;
using OrderManagementAPI.Models;
namespace OrderManagementAPI.Data

{
    public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
    {
        //Table
        public DbSet<Order> Orders => Set<Order>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderID);
                // cannot be null and must not exceed max lenght
                // Adding validation at database level to prevent duplicates
                entity.Property(o => o.CustomerName).IsRequired().HasMaxLength(250);

                entity.HasIndex(o => new { o.CustomerName, o.OrderDate, o.OrderValue })
                 .IsUnique();
            });
        }
    }
}
