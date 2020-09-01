using EasyBilling.Data;
using EasyBilling.Helpers;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Services
{
    public class DbInitializer
    {
        private readonly BillingDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<Models.Pocos.Role> _roleManager;

        public DbInitializer(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            _userManager = sp.GetRequiredService<UserManager<IdentityUser>>();
            _roleManager = sp.GetRequiredService<RoleManager<Models.Pocos.Role>>();
            _dbContext = sp.GetRequiredService<BillingDbContext>();
        }
        public async Task InitializeAsync()
        {
            await ControllersNamesInitAsync();
            await DeviceStatesInitAsync();
            await DeviceTypesInitAsync();
            await TariffsInitAsync();
            await RolesInitAsync();
            await AccessRightsInitAsync();
            await UsersInitAsync();
            await ClientsInitAsync();
        }

        /// <summary>
        /// Инициализация пользователей
        /// </summary>
        /// <returns></returns>
        private async Task UsersInitAsync()
        {
            if (_userManager.Users.Count() == 0)
            {
                IdentityUser admin = null;
                Profile adminProfile = new Profile()
                {
                    FirstName = "Администратор",
                    SecondName = "Биллинга",
                    Address = "Пушкина 9-15",
                    Tariff = await _dbContext.Tariffs.FirstOrDefaultAsync(),
                    Account = admin = new IdentityUser()
                    {
                        UserName = "admin",
                        Email = "admin@localhost",
                        PhoneNumber = "099-999-99-99",
                        EmailConfirmed = true,
                        LockoutEnabled = true
                    }
                };

                var result = await _userManager.CreateAsync(admin, "AQeT.5*gehWqeAh");
                if (result.Succeeded)
                {
                    var adminRole = Helpers.Role.admin.ToString();
                    await _userManager.AddToRoleAsync(admin, adminRole);
                    await _dbContext.Profiles.AddAsync(adminProfile);
                    await _dbContext.SaveChangesAsync();
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
        private async Task RolesInitAsync()
        {
            if (_roleManager.Roles.Count() == 0)
            {
                var dic = await RoleHelper.GetRolesAsync();
                foreach (var item in dic)
                {
                    await _roleManager.CreateAsync(new Models.Pocos.Role()
                    {
                        Name = item.Key,
                        NormalizedName = item.Key.ToUpper(),
                        LocalizedName = item.Value
                    });
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Инициализация прав доступа
        /// </summary>
        /// <returns></returns>
        private async Task AccessRightsInitAsync()
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
                var tmp = Helpers.Role.admin.ToString();
                var adminRole = await  _roleManager.FindByNameAsync(tmp);
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(cassaCtrl)),
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(usersCtrl)),
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(clientCtrl)),
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(deviceCtrl)),
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(accessRightsCtrl)),
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(tariffCtrl)),
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(apiKeyCtrl)),
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(eventCtrl)),
                    IsAvailable = true,
                    Role = adminRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(financialOperationsCtrl)),
                    IsAvailable = true,
                    Role = adminRole
                });
                #endregion
                #region operator
                var operatorRole = await _roleManager.FindByNameAsync(
                    Helpers.Role.@operator.ToString());
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(usersCtrl)),
                    IsAvailable = true,
                    Role = operatorRole
                });
                await _dbContext.AccessRights.AddAsync(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(clientCtrl)),
                    IsAvailable = true,
                    Role = operatorRole
                });
                #endregion
                #region casher
                var casherRole = await _roleManager.FindByNameAsync(
                    Helpers.Role.casher.ToString());
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(cassaCtrl)),
                    IsAvailable = true,
                    Role = casherRole
                });
                #endregion
                #region client
                var clientRole = await _roleManager.FindByNameAsync(
                    Helpers.Role.casher.ToString());
                _dbContext.AccessRights.Add(new AccessRight()
                {
                    Controller = await _dbContext.ControllersNames.FirstOrDefaultAsync(c => c.Name.Equals(clientCtrl)),
                    IsAvailable = true,
                    Role = clientRole
                });
                #endregion

                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Инициализация списка имён контроллеров
        /// </summary>
        /// <returns></returns>
        private async Task ControllersNamesInitAsync()
        {
            if (_dbContext.ControllersNames.Count() == 0)
            {
                var dic = await ControllerHelper.GetControllersNamesAsync();
                foreach (var item in dic)
                {
                    await _dbContext.ControllersNames.AddAsync(new ControllerName()
                    {
                        Name = item.Key,
                        LocalizedName = item.Value
                    });
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Инициализация типов устройства
        /// </summary>
        /// <returns></returns>
        private async Task DeviceTypesInitAsync()
        {
            if (_dbContext.DeviceTypes.Count() == 0)
            {
                var dic = await DeviceHelper.GetDeviceTypes();
                foreach (var item in dic)
                {
                    await _dbContext.DeviceTypes.AddAsync(new DeviceType()
                    {
                        Name = item.Key,
                        LocalizedName = item.Value
                    });
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Инициализация состояний устройства
        /// </summary>
        /// <returns></returns>
        private async Task DeviceStatesInitAsync()
        {
            if (_dbContext.DeviceStates.Count() == 0)
            {
                var dic = await DeviceHelper.GetDeviceStates();
                foreach (var item in dic)
                {
                    await _dbContext.DeviceStates.AddAsync(new DeviceState()
                    {
                        Name = item.Key,
                        LocalizedName = item.Value
                    });
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        ///  Инициалищация тарифов
        /// </summary>
        /// <returns></returns>
        private async Task TariffsInitAsync()
        {
            if (_dbContext.Tariffs.Count() == 0)
            {
                await _dbContext.Tariffs.AddAsync(new Tariff()
                {
                    Name = "Стандартный",
                    Price = 90.0,
                    IsEnabled = true,
                    AmounfOfDays = 28,
                    AmountOfTraffic = 0,
                    BandwidthInput = 100000,
                    BandwidthOutput = 100000
                });
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        ///  Инициалищация базы данных абонентов
        /// </summary>
        /// <returns></returns>
        private async Task ClientsInitAsync()
        {

        }
    }
}