using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPCORESimpleAppWithTests.Models
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(o =>
            {
                o.Property(e => e.Id).ValueGeneratedOnAdd();
                o.HasKey(e => e.Id);
                o.Property(e => e.ClientName);
                o.Property(e => e.OrderTime);
                o.Property(e => e.Amount);
                o.Property(e => e.DeliveryAddress);
                o.HasOne(e => e.Product).WithMany(p => p.Orders).HasForeignKey(e => e.ProductId);
            });
            modelBuilder.Entity<Product>(p =>
            {
                p.Property(e => e.Id).ValueGeneratedOnAdd();
                p.HasKey(e => e.Id);
                p.Property(e => e.ProductName);
                p.Property(e => e.Category);
                p.Property(e => e.AmountInStock);
                p.Property(e => e.Price);
                p.HasMany(e => e.Orders).WithOne();
            });
        }
    }
}
