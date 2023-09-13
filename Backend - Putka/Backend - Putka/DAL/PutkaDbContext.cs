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
    }
}
