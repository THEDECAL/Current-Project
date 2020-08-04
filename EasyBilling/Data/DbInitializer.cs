using EasyBilling.Models.Enums;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EasyBilling.Data
{
    public class DbInitializer
    {
        const 

        static private readonly DbInitializer _dbInit;

        private readonly IServiceScope _scope;
        private readonly UserManager<IdentityAccount> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly BillingDbContext _dbContext;
        private readonly IConfiguration _config;
        static public DbInitializer GetInstance(IHost host) => _dbInit ?? new DbInitializer(host);

        private DbInitializer(IHost host)
        {
            using (_scope = host.Services.CreateScope()) {
                _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<IdentityAccount>>();
                var rm = _scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var bc = _scope.ServiceProvider.GetRequiredService<BillingDbContext>();
                var appBuilder = new ConfigurationBuilder().AddJsonFile("", false, false);
            }
        }
        public async Task InitializeAsync()
        {
            await RolesInitializeAsync();
            await UsersInitializeAsync();
            await AccessRightsInitializeAsync();
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
                IsAccessing = true,
            }); ;
        }
    }
}