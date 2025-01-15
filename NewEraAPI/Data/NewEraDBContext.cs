using Microsoft.EntityFrameworkCore;
using NewEraAPI.Models;

namespace NewEraAPI.Data
{
    public class NewEraDBContext : DbContext
    {
        public NewEraDBContext(DbContextOptions<NewEraDBContext> options) :
        base(options)
        { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
            .HasMany(Category => Category.Products)
            .WithOne(Product => Product.Category)
            .HasForeignKey(Product => Product.CategoryID);

            modelBuilder.Entity<Order>()
                .HasMany(order => order.OrderItems)
                .WithOne(orderItem => orderItem.Order)
                .HasForeignKey(orderItem => orderItem.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(orderItem => orderItem.Product)
                .WithMany()
                .HasForeignKey(orderItem => orderItem.ProductId);

            base.OnModelCreating(modelBuilder);
        }

    }
}
