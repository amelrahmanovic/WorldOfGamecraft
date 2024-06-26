﻿using System.ComponentModel.DataAnnotations;

namespace AccountService.Models.VM
{
    public class RegisterModelVM
    {
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserRole { get; set; }
    }
}
