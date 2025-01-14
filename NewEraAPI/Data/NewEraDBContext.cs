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

    }
}
