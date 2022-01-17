using Microsoft.EntityFrameworkCore;

namespace SecurityTemplateSolution.Models
{
    public class SecurityDbContext : DbContext
    {
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelbuilder) 
        {
            modelbuilder.Entity<User>()
                .HasIndex(p=>p.Login)
                .IsUnique(true);
        }
    }
}
