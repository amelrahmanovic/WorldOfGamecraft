using System.ComponentModel.DataAnnotations;

namespace AccountService.Models.VM
{
    public class LoginModelVM
    {
        [Required(ErrorMessage = "User Name is required")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "Remember me is required")]
        public bool RememberMe { get; set; }
    }
}
