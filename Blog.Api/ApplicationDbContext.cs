using Microsoft.EntityFrameworkCore;

namespace Blog.Api
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BlogPost> Posts { get; set; }
    }
}
