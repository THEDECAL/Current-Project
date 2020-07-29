using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Data
{
    static public class RolesHelper
    {
        public enum Role { Administrator, Operator, Casher, Client };
        static string[] _roles = { "Администратор", "Оператор", "Кассир", "Клиент" };

        static public string GetRoleName(Role role) => _roles[(int)role];
        static public IdentityRole GetRole(Role role) => new IdentityRole(GetRoleName(role));
        static public IdentityRole[] GetRoles() => _roles.Select(role => new IdentityRole(role)).ToArray();
    }
}
