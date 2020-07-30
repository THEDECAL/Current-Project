using EasyBilling.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Data
{
    public class DbInitializer
    {
        private readonly UserManager<IdentityAccount> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(UserManager<IdentityAccount> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            RolesInitializeAsync().Wait();
            UsersInitializeAsync().Wait();
        }
        /// <summary>
        /// Инициализация пользователей
        /// </summary>
        /// <returns></returns>
        private async Task UsersInitializeAsync()
        {
            if (_userManager.Users.Count() == 0)
            {
                var admin = new IdentityAccount()
                {
                    UserName = "admin",
                    Email = "admin@localhost",
                    PhoneNumber = "099-999-99-99",
                    IsEnabled = true,
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                    Profile = new Profile()
                    {
                        FirstName = "Администратор",
                        SecondName = "Биллинга",
                        Address = "Пушкина 9-15",
                        DateOfCreation = DateTime.Now
                    }
                };
                var result = await _userManager.CreateAsync(admin, @"AQeT.5*gehWqeAh");
                if (result.Succeeded)
                {
                    var adminRole = RolesHelper.Role.admin.ToString();
                    await _userManager.AddToRoleAsync(admin, adminRole);
                }
            }
        }
        /// <summary>
        /// Инициализация ролей
        /// </summary>
        /// <returns></returns>
        private async Task RolesInitializeAsync()
        {
            if (_roleManager.Roles.Count() == 0)
            {
                var roles = RolesHelper.GetRoles();
                foreach (var item in roles)
                {
                    await _roleManager.CreateAsync(item);
                }
            }
        }
    }
}