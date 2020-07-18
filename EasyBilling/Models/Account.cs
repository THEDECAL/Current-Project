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
    }
}   