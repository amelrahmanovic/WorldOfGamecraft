using CombatService.Models;
using Microsoft.EntityFrameworkCore;

namespace CombatService
{
    public class AppDbContex : DbContext
    {
        public AppDbContex(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
