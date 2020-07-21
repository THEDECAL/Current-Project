using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models
{
    public class Account : IdentityUser
    {
        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
        public DateTime LastLogin { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}