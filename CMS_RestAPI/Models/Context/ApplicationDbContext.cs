using Microsoft.EntityFrameworkCore;

namespace CMS_RestAPI.Models.Context
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options) {}
        public DbSet<Category>Categories { get; set; }
        public DbSet<AppUser> Users { get; set; }
    }
}
