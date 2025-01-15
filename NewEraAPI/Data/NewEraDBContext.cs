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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryID);

            base.OnModelCreating(modelBuilder);
        }

    }
}
