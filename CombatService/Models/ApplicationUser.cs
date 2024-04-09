using System.ComponentModel.DataAnnotations;

namespace CombatService.Models
{
    public class ApplicationUser
    {
        [Key]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
