using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Models;
using EasyBilling.Models.Pocos;
using System.Diagnostics.CodeAnalysis;

namespace EasyBilling.Helpers
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
        static Dictionary<string,string> _roleNames = new Dictionary<string, string>()
        {
            { "admin", "Администратор" },
            { "operator", "Оператор" },
            { "casher", "Кассир" },
            { "client", "Клиент" }
        };

        static public string GetRoleLocalizedName(Role role) => _roleNames.ElementAt((int)role).Value;
        static public string GetRoleLocalizedName([NotNull] string role) => _roleNames.GetValueOrDefault(role);
        static public IdentityRole GetRole(Role role) =>
            new IdentityRole(role.ToString());

        static async public Task<IdentityRole[]> GetRolesAsync() =>
            await Task.Run(() =>
            {
                var enumValues = Enum.GetValues(typeof(Role))
                    .OfType<Role>().ToList();
                return enumValues.Select(role => GetRole(role)).ToArray();
            });
    }
}
