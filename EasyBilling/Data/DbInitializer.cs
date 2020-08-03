using EasyBilling.Models.Enums;
using EasyBilling.Models.Pocos;
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
        private readonly BillingDbContext _dbContext;
        public DbInitializer(UserManager<IdentityAccount> userManager,
            RoleManager<IdentityRole> roleManager,
            BillingDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }
        public void Initialize()
        {
            RolesInitializeAsync().Wait();
            UsersInitializeAsync().Wait();
            AccessRightsInitializeAsync().Wait();
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
                    var adminRole = Role.admin.ToString();
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
                var roles = RoleHelper.GetRoles();
                foreach (var item in roles)
                {
                    await _roleManager.CreateAsync(item);
                }
            }
        }
        private async Task AccessRightsInitializeAsync()
        {
            //admin
            _dbContext.AccessToControllers.Add(new AccessToController()
            {
                Role = new IdentityRole(Role.admin.ToString()),
                IsAccessing = true,
                
            }); ;
        }
    }
}