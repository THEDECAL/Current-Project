using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Models;

namespace EasyBilling.Data
{
/*    [Flags]
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
        };*/

     /*   public string GetRoleName(Role role) => _roles[(int)role];
        public IdentityRole GetRole(Role role) => new IdentityRole(role.ToString());
        public IdentityRole[] GetRoles()
        {
            var enumValues = Enum.GetValues(typeof(Role)).OfType<Role>().ToList();
            return enumValues.Select(role => GetRole(role)).ToArray();
        }
    }*/
}
