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
        UserManager<IdentityAccount> _userManager;
        public DbInitializer(UserManager<IdentityAccount> userManager)
        {
            _userManager = userManager;
        }
        /// <summary>
        /// Инициализация пользователей
        /// </summary>
        /// <returns></returns>
        public async Task UsersInitializeAsync()
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
                    /*await _userManager.AddToRoleAsync(admin, RolesHelper.GetRoleName(RolesHelper.Role.Administrator));*/
                }
                else throw new Exception($"User ${admin.UserName} is not created");
            }
            else throw new Exception("Users is exist.");
        }
    }
}