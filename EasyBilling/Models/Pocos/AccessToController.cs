using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models.Pocos
{
    public class AccessToController
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public IdentityRole Role { get; set; }
        public string ComponentsPermissionsJson { get; set; }
        public bool IsAccessing { get; set; }
    }
}