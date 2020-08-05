using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models.Pocos
{
    public class IdentityGroup : IdentityRole
    {
        public string LocalizedName { get; set; }
        public IdentityGroup(string role): base(role){}
    }
}
