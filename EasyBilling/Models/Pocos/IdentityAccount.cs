using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace EasyBilling.Models.Pocos
{
    public class IdentityAccount : IdentityUser
    {
        [Required]
        public Profile Profile { get; set; }
        public DateTime LastLogin { get; set; }
        [Required]
        public bool IsEnabled { get; set; } = true;
    }
}