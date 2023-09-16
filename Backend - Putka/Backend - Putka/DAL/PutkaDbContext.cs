using Backend___Putka.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend___Putka.DAL
{
    public class PutkaDbContext:DbContext
    {
        public PutkaDbContext(DbContextOptions<PutkaDbContext> options):base(options)
        {
            
        }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductTag>().HasKey(x => new { x.TagId, x.ProductId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
