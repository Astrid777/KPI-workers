using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CustomIdentityApp.Models
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();

        }
        public DbSet<CustomIdentityApp.Models.Department> Department { get; set; }
        public DbSet<CustomIdentityApp.Models.Indicator> Indicator { get; set; }
        public DbSet<CustomIdentityApp.Models.Rating> Ratings { get; set; }
        public DbSet<CustomIdentityApp.Models.News> News { get; set; }

        public DbSet<FileOnDatabaseModel> Files { get; set; }
    }
}