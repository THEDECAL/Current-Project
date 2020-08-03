using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Models.Enums;

namespace EasyBilling.Data
{
    static public class RoleHelper
    {
        static string[] _roles = { "Администратор", "Оператор", "Кассир", "Клиент" };

        static public string GetRoleName(Role role) => _roles[(int)role];
        static public IdentityRole GetRole(Role role) => new IdentityRole(role.ToString());
        static public IdentityRole[] GetRoles()
        {
            var enumValues = Enum.GetValues(typeof(Role)).OfType<Role>().ToList();
            return enumValues.Select(role => GetRole(role)).ToArray();
        }
    }
}
