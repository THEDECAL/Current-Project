using EasyBilling.Helpers;
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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EasyBilling.Data
{
    public class DbInitializer
    {
        private static DbInitializer _dbInit;

        private readonly BillingDbContext _dbContext;
        private readonly UserManager<IdentityAccount> _userMgr;
        private readonly RoleManager<IdentityGroup> _roleMgr;

        private DbInitializer(IHost host)
        {
            using (var scope = host.Services.CreateScope()) {
                var sp = scope.ServiceProvider;
                _userMgr = sp.GetRequiredService<UserManager<IdentityAccount>>();
                _roleMgr = sp.GetRequiredService<RoleManager<IdentityGroup>>();
                _dbContext = sp.GetRequiredService<BillingDbContext>();
            }
        }
        /// <summary>
        /// Получение единственного экземпляра
        /// </summary>
        /// <param name="host"></param>
        /// <returns>Возвращает объект класса</returns>
        public static DbInitializer GetInstance(IHost host) =>
            _dbInit = (_dbInit == null) ? new DbInitializer(host) : _dbInit;

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
                var roles = RoleHelper.GetRoles();
                foreach (var item in roles)
                {
                    await _roleMgr.CreateAsync(item);
                }
            }
        }
        private async Task AccessRightsInitializeAsync()
        {
            using (_dbContext)
            {
                #region admin
                var adminRole = await _roleMgr.FindByNameAsync(
                    Role.admin.ToString());
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.Users,
                    IsAvailable = true,
                    Role = adminRole
                });
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.Client,
                    IsAvailable = true,
                    Role = adminRole
                });
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.Device,
                    IsAvailable = true,
                    Role = adminRole
                });
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.Tariff,
                    IsAvailable = true,
                    Role = adminRole
                });
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.APIKey,
                    IsAvailable = true,
                    Role = adminRole
                });
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.Event,
                    IsAvailable = true,
                    Role = adminRole
                });
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.FinancialOperations,
                    IsAvailable = true,
                    Role = adminRole
                });
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.AccessRights,
                    IsAvailable = true,
                    Role = adminRole
                });
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.Cassa,
                    IsAvailable = true,
                    Role = adminRole
                });
                #endregion

                #region operator
                var operatorRole = await _roleMgr.FindByNameAsync(
                    Role.@operator.ToString());
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.Users,
                    IsAvailable = true,
                    Role = operatorRole
                });
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.Client,
                    IsAvailable = true,
                    Role = operatorRole
                });
                #endregion

                #region casher
                var casherRole = await _roleMgr.FindByNameAsync(
                    Role.casher.ToString());
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.Cassa,
                    IsAvailable = true,
                    Role = casherRole
                });
                #endregion

                #region client
                var clientRole = await _roleMgr.FindByNameAsync(
                    Role.casher.ToString());
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    PageId = (int)PageName.Client,
                    IsAvailable = true,
                    Role = clientRole
                });
                #endregion

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}