using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Models;
using EasyBilling.Models.Pocos;

namespace EasyBilling.Data
{
    [Flags]
    public enum Role
    {
        admin,
        @operator,
        casher,
        client
    };
    static public class RoleHelper
    {
        static string[] _roles =
        {
            "Администратор",
            "Оператор",
            "Кассир",
            "Клиент"
        };

        static public string GetRoleLocalizedName(Role role) => _roles[(int)role];
        static public IdentityGroup GetRole(Role role)
            => new IdentityGroup(role.ToString())
                { LocalizedName = GetRoleLocalizedName(role)};
        static public IdentityGroup[] GetRoles()
        {
            var enumValues = Enum.GetValues(typeof(Role)).OfType<Role>().ToList();
            return enumValues.Select(role => GetRole(role)).ToArray();
        }
    }
}
