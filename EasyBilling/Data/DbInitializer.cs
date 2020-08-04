using EasyBilling.Models;
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
        private static readonly DbInitializer _dbInit;

        private readonly BillingDbContext _dbContext;
        private readonly UserManager<IdentityAccount> _userMgr;
        private readonly RoleManager<IdentityRole> _roleMgr;
        //private readonly IConfiguration _config;

        private DbInitializer(IHost host)
        {
            using (var scope = host.Services.CreateScope()) {
                var sp = scope.ServiceProvider;
                _userMgr = sp.GetRequiredService<UserManager<IdentityAccount>>();
                _roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
                _dbContext = sp.GetRequiredService<BillingDbContext>();
                //var appBuilder = new ConfigurationBuilder().AddJsonFile("", false, false);
            }
        }
        /// <summary>
        /// Получение единственного экземпляра
        /// </summary>
        /// <param name="host"></param>
        /// <returns>Возвращает объект класса</returns>
        public static DbInitializer GetInstance(IHost host) => _dbInit ?? new DbInitializer(host);

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
            if (_userMgr.Users.Count() == 0)
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
                var result = await _userMgr.CreateAsync(admin, @"AQeT.5*gehWqeAh");
                if (result.Succeeded)
                {
                    //var adminRole = Role.admin.ToString();
                    //await _userMgr.AddToRoleAsync(admin, adminRole);
                }
            }
        }
        /// <summary>
        /// Инициализация ролей
        /// </summary>
        /// <returns></returns>
        private async Task RolesInitializeAsync()
        {
            if (_roleMgr.Roles.Count() == 0)
            {
                //var roles = RoleHelper.GetRoles();
                //foreach (var item in roles)
                //{
                //    await _roleMgr.CreateAsync(item);
                //}
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