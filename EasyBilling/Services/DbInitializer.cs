using EasyBilling.Data;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Services
{
    public class DbInitializer //: IDisposable
    {
        private readonly BillingDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            _userManager = sp.GetRequiredService<UserManager<IdentityUser>>();
            _roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
            _dbContext = sp.GetRequiredService<BillingDbContext>();
        }
        public async Task InitializeAsync()
        {
            await Task.Run(async () =>
            {
                await RolesInitializeAsync();
                await AccessRightsInitializeAsync();
                await UsersInitializeAsync();
            });
        }
        /// <summary>
        /// Инициализация пользователей
        /// </summary>
        /// <returns></returns>
        private async Task UsersInitializeAsync()
        {
            if (_userManager.Users.Count() == 0)
            {
                IdentityUser admin = null;
                Profile adminProfile = new Profile()
                {
                    FirstName = "Администратор",
                    SecondName = "Биллинга",
                    Address = "Пушкина 9-15",
                    DateOfCreation = DateTime.Now,
                    BirthDay = new DateTime()
                        .AddDays(23)
                        .AddYears(1990)
                        .AddMonths(8),
                    Account = admin = new IdentityUser()
                    {
                        UserName = "admin",
                        Email = "admin@localhost",
                        PhoneNumber = "099-999-99-99",
                        EmailConfirmed = true,
                        LockoutEnabled = true
                    }
                };

                var result = await _userManager.CreateAsync(admin, @"AQeT.5*gehWqeAh");
                if (result.Succeeded)
                {
                    var adminRole = Role.admin.ToString();
                    await _userManager.AddToRoleAsync(admin, adminRole);
                }
                else
                {
                    Console.WriteLine($"Роль {adminProfile}" +
                        $"не связалась с {admin.UserName}," + 
                        "произошла ошибка.");
                }
            }
        }
        /// <summary>
        /// Инициализация ролей
        /// </summary>
        /// <returns></returns>
        private async Task RolesInitializeAsync()
        {
            //if (_roleManager.Roles.Count() == 0)
            if (_dbContext.Roles.Count() == 0)
            {
                 var roles = await RoleHelper.GetRolesAsync();
                var roleLst = new List<IdentityRole>();

                foreach (var item in roles)
                {
                    if (item != null)
                    {
                        roleLst.Add(item);
                        await _dbContext.AddAsync(item);
                        //await _roleManager.CreateAsync(item);
                    }
                }
                await _dbContext.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Инициализация прав доступа
        /// </summary>
        /// <returns></returns>
        private async Task AccessRightsInitializeAsync()
        {
            if (_dbContext.AccessRights.Count() == 0)
            {
                const string cassaCtrl = "Cassa";
                const string usersCtrl = "Users";
                const string clientCtrl = "Client";
                const string deviceCtrl = "Device";
                const string accessRightsCtrl = "AccessRights";
                const string tariffCtrl = "Tariff";
                const string apiKeyCtrl = "APIKey";
                const string eventCtrl = "Event";
                const string financialOperationsCtrl = "FinancialOperations";
                #region admin
                var adminRole = await  _roleManager.FindByNameAsync(
                    Role.admin.ToString());
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    ControllerName = usersCtrl,
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    ControllerName = clientCtrl,
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    ControllerName = accessRightsCtrl,
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    ControllerName = tariffCtrl,
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    ControllerName = apiKeyCtrl,
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    ControllerName = eventCtrl,
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    ControllerName = financialOperationsCtrl,
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    ControllerName = deviceCtrl,
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    ControllerName = clientCtrl,
                    IsAvailable = true,
                    Role = adminRole
                });
                #endregion
                #region operator
                var operatorRole = await _roleManager.FindByNameAsync(
                    Role.@operator.ToString());
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    ControllerName = usersCtrl,
                    IsAvailable = true,
                    Role = operatorRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    ControllerName = clientCtrl,
                    IsAvailable = true,
                    Role = operatorRole
                });
                #endregion
                #region casher
                var casherRole = await _roleManager.FindByNameAsync(
                    Role.casher.ToString());
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    ControllerName = cassaCtrl,
                    IsAvailable = true,
                    Role = casherRole
                });
                #endregion
                #region client
                var clientRole = await _roleManager.FindByNameAsync(
                    Role.casher.ToString());
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    ControllerName = clientCtrl,
                    IsAvailable = true,
                    Role = clientRole
                });
                #endregion

                await _dbContext.SaveChangesAsync();
            }
        }
        //public async void Dispose()
        //{
        //    await _dbContext.DisposeAsync();
        //    await Task.Run(() =>
        //    {
        //        _roleManager.Dispose();
        //        _userManager.Dispose();
        //    });
        //}
        /// <summary>
        ///  Инициалищация базы данных абонентов
        /// </summary>
        /// <returns></returns>
        //private async Task ClientsInitializeAsync()
        //{

        //}
        /// <summary>
        ///  Инициалищация тарифов
        /// </summary>
        /// <returns></returns>
        //private async Task TariffsInitializeAsync()
        //{

        //}
    }
}