using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models;
using EasyBilling.Models.Entities;
using EasyBilling.Services;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Controllers
{
    [DisplayName("Пользователи")]
    public class UsersController : ExtController<Profile>
    {
        private TariffRegulator _tariffRegulator;
        public UsersController(BillingDbContext dbContext,
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            TariffRegulator tariffRegulator,
            IServiceScopeFactory scopeFactory)
            : base(dbContext, roleManager, userManager, scopeFactory)
        { _tariffRegulator = tariffRegulator; }

        //[HttpGet]
        //[DisplayName("Список")]
        //public override async Task<IActionResult> Index()
        //{
        //    return await Task.Run(() =>
        //    {
        //        var dvm = new TableDataViewModel<Profile>(_scopeFactory,
        //            urlPath: HttpContext.Request.Path,
        //            settings: Settings,
        //            includeFields: new string[]
        //            {
        //                nameof(Profile.Tariff),
        //                nameof(Profile.User)
        //            },
        //            excludeFields: new string[]
        //            {
        //                nameof(Profile.Patronymic),
        //                nameof(Profile.Comment),
        //                nameof(Profile.BalanceOfCash),
        //                nameof(Profile.LastOfLogin),
        //                nameof(Profile.DateOfUpdate),
        //                nameof(Profile.StartUsedOfTariff),
        //                nameof(Profile.TrafficUsed),
        //                nameof(Profile.CustomProfileField1),
        //                nameof(Profile.CustomProfileField2),
        //                nameof(Profile.CustomProfileField3)
        //            }
        //        );

        //        return View("CustomIndex", model: dvm);
        //    });
        //}

        [DisplayName(("Форма добавить/изменить"))]
        [HttpGet]
        public async Task<IActionResult> AddUpdateForm(int? id = null)
        {
            return await Task.Run(async () =>
            {
                ViewData["ActionPage"] = nameof(Add);

                var model = new Profile();
                if (id != null)
                {
                    model = await DbContext.Profiles
                        .Include(o => o.User)
                        .Include(o => o.Tariff)
                        .FirstOrDefaultAsync(o => o.Id.Equals(id));
                        if (model == null)
                        model = new Profile();
                    else
                        ViewData["ActionPage"] = nameof(Change);
                }

                return View(nameof(AddUpdateForm), model: model);
            });
        }

        [DisplayName("Создать")]
        [HttpPost]
        public async Task<IActionResult> Create(Profile obj, string roleName, int tariffId)
        {
            await ServerSideValidation(obj, roleName, tariffId);
            if (ModelState.IsValid)
            {
                obj.Tariff = await DbContext.Tariffs
                    .FirstOrDefaultAsync(t => t.Id.Equals(tariffId));

                var result = await UserMngr.CreateAsync(obj.User);
                if (result.Succeeded)
                {
                    var role = await RoleMngr.FindByNameAsync(roleName);
                    await UserMngr.AddToRoleAsync(obj.User, role.Name);
                }

                await DbContext.Profiles.AddAsync(obj);
                await DbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        [DisplayName("Изменить")]
        [HttpPost]
        public async Task<IActionResult> Update(Profile obj, string roleName, int tariffId)
        {
            await ServerSideValidation(obj, roleName, tariffId);
            if (ModelState.IsValid)
            {
                var accountExisting = await DbContext.Users
                    .FirstOrDefaultAsync(u => u.Id.Equals(obj.User.Id));
                var profileExisting = await DbContext.Profiles.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id.Equals(obj.Id));
                accountExisting.UserName = obj.User.UserName;
                accountExisting.Email = obj.User.Email;
                obj.User = accountExisting;
                obj.Tariff = await DbContext.Tariffs
                    .FirstOrDefaultAsync(t => t.Id.Equals(tariffId));

                var role = await RoleMngr.FindByNameAsync(roleName);
                await UserMngr.RemoveFromRoleAsync(obj.User, role.Name);
                await UserMngr.AddToRoleAsync(obj.User, role.Name);

                await Task.Run(() =>
                {
                    if (profileExisting.IsEnabled != obj.IsEnabled)
                    {
                        _tariffRegulator.StartToUseOfTariff(obj);
                    }

                    obj.DateOfUpdate = DateTime.Now;
                    DbContext.Update(obj);
                    DbContext.SaveChanges();
                });

                return RedirectToAction("Index");
            }

            return await AddUpdateForm(obj.Id);
        }

        [DisplayName("Удалить")]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id = null)
        {
            var profile = await DbContext.Profiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id.Equals(id.Value));

            await Task.Run(() =>
            {
                if (profile != null)
                {
                    DbContext.Users.Remove(profile.User);
                    DbContext.Profiles.Remove(profile);
                    DbContext.SaveChanges();
                }
            });

            return RedirectToAction("Index");
        }

        public async Task ServerSideValidation(Profile obj, string roleName, int tariffId)
        {
            TryValidateModel(obj);
            ModelState.Remove("Tariff.Name");

            var isUserNameExist = await DbContext.Users
                .AnyAsync(u => u.UserName.Equals(obj.User.UserName));
            var isEmailExist = await DbContext.Users
                    .AnyAsync(u => u.Email.Equals(obj.User.Email));
            if (ActionName.Equals(nameof(Add)))
            {
                if (isUserNameExist)
                { ModelState.AddModelError("Account.UserName", "Введённый логин уже существует, выберите другой"); }
                if (isUserNameExist)
                { ModelState.AddModelError("Account.Email", "Введённый почтовый адрес уже сущесвует, выберите другой"); }
            }
            else
            {
                var oldAccount = await DbContext.Users
                    .FirstOrDefaultAsync(u => u.Id.Equals(obj.User.Id));
                if (oldAccount != null)
                {
                    if (!oldAccount.UserName.Equals(obj.User.UserName) && isUserNameExist)
                    { ModelState.AddModelError("Account.UserName", "Введённый логин уже существует, выберите другой"); }
                    if (!oldAccount.Email.Equals(obj.User.Email) && isEmailExist)
                    { ModelState.AddModelError("Account.Email", "Введённый почтовый адрес уже сущесвует, выберите другой"); }
                }
                else
                { ModelState.AddModelError("", "Аккаунт профиля не найден"); }
            }

            var pattern = @"^(?=.*[a-z0-9])[a-z][a-z\d.-]{0,19}$";
            var regexCheck = Regex.IsMatch(obj.User.UserName, pattern);
            if (!regexCheck)
            { ModelState.AddModelError("Account.UserName",
                "Логин должен соответствовать латинским маленьким буквам и быть не длинее 19 символов"); }

            var isTariffExist = await DbContext.Tariffs
                .AnyAsync(t => t.Id.Equals(tariffId));
            if (!isTariffExist)
            { ModelState.AddModelError("Tariff", "Выбранного тарифа не существует"); }

            var isRoleExist = await DbContext.Roles
                .AnyAsync(r => r.Name.Equals(roleName));
            if(!isRoleExist)
            { ModelState.AddModelError("", "Выбранной роли не существует"); }
        }
    }
}
