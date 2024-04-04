using CharacterService.Models;
using Microsoft.EntityFrameworkCore;

namespace CharacterService
{
    public class AppDbContex :DbContext
    {
        public AppDbContex(DbContextOptions options) : base(options)
        {
                
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CharacterItem>().HasKey(sc => new { sc.CharacterId, sc.ItemId });
            builder.Entity<Character>().HasIndex(x => x.Name).IsUnique();

            builder.Entity<Class>().HasData(
                new Class() { Name= "Warrior", Description = "Warrior in life", Id=1 },
                new Class() { Name= "Rogue", Description = "Rogue in life", Id = 2 },
                new Class() { Name= "Mage", Description = "Mage in life", Id = 3 },
                new Class() { Name= "Priest", Description = "Priest in life", Id = 4 }
                );

            base.OnModelCreating(builder);
        }
        public DbSet<Item> Item { get; set; }
        public DbSet<Class> Class { get; set; }
        public DbSet<Character> Character { get; set; }
        public DbSet<CharacterItem> CharacterItem { get; set; }
    }
}
