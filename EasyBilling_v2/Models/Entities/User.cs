using EasyBilling.Attributes;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models.Entities
{
    public class User : IdentityUser
    {
        //[DisplayName("Профиль")]
        //public Profile Profile { get; set; }
        //public int ProfileId { get; set; }

        [DisplayName("Роль")]
        public Role Role { get; set; }
        [NoShowInTable]
        public string RoleId { get; set; }

        public override bool Equals(object obj) => (!string.IsNullOrWhiteSpace((obj as User).Id)) && (obj as User).Id.Equals(Id);
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => $"{(string.IsNullOrWhiteSpace(UserName) ? "" : "(" + UserName + ")")} {{Role}}";
    }
}
