using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models.Pocos
{
    public class Role : IdentityRole
    {
        public ControllerName DefaultControllerName { get; set; }
        public string LocalizedName { get; set; }

        public override string ToString() => (string.IsNullOrWhiteSpace(LocalizedName)) ? Name : LocalizedName;
    }
}
