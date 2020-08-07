using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyBilling.Services
{
    public class AccessRightsManager: IDisposable
    {
        private readonly BillingDbContext _dbContext;
        private readonly UserManager<IdentityAccount> _userManager;
        public AccessRightsManager(BillingDbContext dbContext, UserManager<IdentityAccount> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public async Task<AccessRight[]> GetAllAsync()
        {
            using (_dbContext)
            {
                return await _dbContext.AccessRights
                    .Include(ar => ar.Role).ToArrayAsync();
            }
        }

        private async Task<IdentityAccount> GetAccountAsync([NotNull] string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                return await _userManager.FindByNameAsync(userName);
            }
            else throw new ArgumentNullException();
        }

        private async Task<IList<string>> GetRolesAsync([NotNull] IdentityAccount acc)
            => await _userManager.GetRolesAsync(acc);
        private async Task<IList<string>> GetRolesAsync([NotNull] string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                IdentityAccount acc = await GetAccountAsync(userName);
                if (acc != null)
                {
                    var roles = await GetRolesAsync(acc);
                    return roles;
                }
                else return null;
            }
            throw new ArgumentNullException();
        }

        public async Task<AccessRight[]> GetRights([NotNull] string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                var roles = await GetRolesAsync(userName);

                if (roles != null && roles.Count > 0)
                {
                    return await _dbContext.AccessRights
                        .Include(ar => ar.Role)
                        .Where(ar => ar.Role.Name.Equals(roles[0]))
                        .ToArrayAsync();
                }
                return null;
            }
            else throw new ArgumentNullException();
        }

        public async Task<AccessRight> GetRights([NotNull] string userName, [NotNull] string controllerName)
        {
            if (!string.IsNullOrWhiteSpace(userName) &&
                !string.IsNullOrWhiteSpace(controllerName))
            {
                var roles = await GetRolesAsync(userName);

                if (roles != null && roles.Count > 0)
                {
                    var rights = await _dbContext.AccessRights
                        .Include(ar => ar.Role)
                        .Where(ar => ar.Role.Name.Equals(roles[0]) &&
                            ar.ControllerName.Equals(controllerName))
                        .FirstOrDefaultAsync();
                    return rights;
                }
                else return null;
            }
            else throw new ArgumentNullException();
        }

        public async Task<string[]> GetMenuItemsByRole([NotNull] string userName)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                var roles = await GetRolesAsync(userName);
                if (roles != null && roles.Count > 0)
                {
                    //var cntrlList = Assembly.GetAssembly(typeof(CustomController)).GetTypes()
                    //    .Where(t => t.IsSubclassOf(typeof(CustomController)) &&
                    //        t.GetCustomAttribute(typeof(NoShowToMenuAttribute)) == null) //Не выберать контроллеры с аттрибутом не показывать в меню
                    //    .Select(t =>
                    //    {
                    //        return t.GetCustomAttributes(typeof(DisplayNameAttribute))
                    //            .FirstOrDefault() as DisplayNameAttribute;
                    //    });

                    //Выбераем контроллеры в соотвествии с правами для пользователя
                    var cntrlByRole = _dbContext.AccessRights
                        .Include(ar => ar.Role)
                        .Where(ar => ar.Role.Name.Equals(roles[0]))
                        .Select(ar => ar.ControllerName).ToArray();

                    foreach (var item in cntrlByRole)
                    {
                        var type = Type.GetType($"{item}Controller");
                    }

                    var menulst = cntrlByRole.Where(c =>
                    {
                        var type = Type.GetType($"{c}controller");

                        return true;
                    }).ToArray();

                    return menulst;
                }
                else return null;
            }
            else throw new ArgumentNullException();
        }

        public async void Dispose()
        {
            await _dbContext.DisposeAsync();
            _userManager.Dispose();
        }
    }
}
