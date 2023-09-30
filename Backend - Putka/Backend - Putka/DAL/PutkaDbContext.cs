using Backend___Putka.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend___Putka.DAL
{
    public class PutkaDbContext:IdentityDbContext
    {
        public PutkaDbContext(DbContextOptions<PutkaDbContext> options):base(options)
        {
            
        }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<Weight> Weights { get; set; }
        public DbSet<ProductWeight> ProductWeights { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductTag>().HasKey(x => new { x.TagId, x.ProductId });
            modelBuilder.Entity<ProductWeight>().HasKey(x => new { x.WeightId, x.ProductId });

            modelBuilder.Entity<Setting>().HasKey(x => x.Key);


            base.OnModelCreating(modelBuilder);
        }
    }
}
