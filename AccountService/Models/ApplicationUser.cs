using Microsoft.AspNetCore.Identity;

namespace AccountService.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}
