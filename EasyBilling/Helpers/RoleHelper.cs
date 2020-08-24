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
        //static public string GetRoleLocalizedName(Role role) => _roleNames.ElementAt((int)role).Value;
        //static public string GetRoleLocalizedName([NotNull] string role) => _roleNames.GetValueOrDefault(role);
        //static public IdentityRole GetRole(Role role) =>
        //    new IdentityRole(role.ToString()) { NormalizedName = role.ToString().ToUpper() };

        static async public Task<Dictionary<string, string>> GetRolesAsync() =>
            await Task.Run(() => new Dictionary<string, string>()
            {
                { Role.admin.ToString(), "Администратор" },
                { Role.@operator.ToString(), "Оператор" },
                { Role.casher.ToString(), "Кассир" },
                { Role.client.ToString(), "Клиент" }
            });
    }
}