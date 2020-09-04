﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models;
using EasyBilling.Models.Pocos;
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
    public class UsersController : CustomController
    {
        private UserManager<IdentityUser> _userManager;

        public UsersController(BillingDbContext dbContext,
            RoleManager<Models.Pocos.Role> roleManager,
            UserManager<IdentityUser> userManager,
            IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, scopeFactory)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [DisplayName("Список")]
        public override async Task<IActionResult> Index(ControlPanelSettings settings = null)
        {
            return await Task.Run(() =>
            {
                var dvm = new DataViewModel<Profile>(_scopeFactory,
                    controllerName: ViewData["ControllerName"] as string,
                    settings: settings,
                    includeFields: new string[]
                    {
                        nameof(Profile.Tariff),
                        nameof(Profile.Account)
                    },
                    excludeFields: new string[]
                    {
                        nameof(Profile.Patronymic),
                        nameof(Profile.Comment),
                        nameof(Profile.AmountOfCash),
                        nameof(Profile.LastLogin),
                        nameof(Profile.DateOfUpdate),
                        nameof(Profile.DateBeginOfUseOfTarrif),
                        nameof(Profile.UsedTraffic),
                        nameof(Profile.CustomProfileField1),
                        nameof(Profile.CustomProfileField2),
                        nameof(Profile.CustomProfileField3)
                    }
                );

                return View("CustomIndex", model: dvm);
            });
        }

        [DisplayName(("Добавить-Изменить"))]
        [HttpGet]
        public async Task<IActionResult> AddUpdateForm(int? id = null)
        {
            return await Task.Run(async () =>
            {
                ViewData["ActionPage"] = nameof(Create);

                var model = new Profile();
                if (id != null)
                {
                    model = await _dbContext.Profiles
                        .Include(o => o.Account)
                        .Include(o => o.Tariff)
                        .FirstOrDefaultAsync(o => o.Id.Equals(id));
                    if (model == null)
                        model = new Profile();
                    else
                        ViewData["ActionPage"] = nameof(Update);
                }

                return View(nameof(AddUpdateForm), model: model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Profile obj, string roleName, int tariffId)
        {
            await ServerSideValidation(obj, roleName, tariffId);
            if (ModelState.IsValid)
            {
                obj.Tariff = await _dbContext.Tariffs
                    .FirstOrDefaultAsync(t => t.Id.Equals(tariffId));

                var result = await _userManager.CreateAsync(obj.Account);
                if (result.Succeeded)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    await _userManager.AddToRoleAsync(obj.Account, role.Name);
                }

                await _dbContext.Profiles.AddAsync(obj);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Profile obj, string roleName, int tariffId)
        {
            await ServerSideValidation(obj, roleName, tariffId);
            if (ModelState.IsValid)
            {
                var accountExisting = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id.Equals(obj.Account.Id));
                accountExisting.UserName = obj.Account.UserName;
                accountExisting.Email = obj.Account.Email;
                obj.Account = accountExisting;
                obj.Tariff = await _dbContext.Tariffs
                    .FirstOrDefaultAsync(t => t.Id.Equals(tariffId));

                var role = await _roleManager.FindByNameAsync(roleName);
                await _userManager.RemoveFromRoleAsync(obj.Account, role.Name);
                await _userManager.AddToRoleAsync(obj.Account, role.Name);

                await Task.Run(() =>
                {
                    _dbContext.Users.Update(obj.Account);
                    _dbContext.Profiles.Update(obj);
                    _dbContext.SaveChanges();
                });

                return RedirectToAction("Index");
            }

            return await AddUpdateForm(obj.Id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id = null)
        {
            var profile = await _dbContext.Profiles
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.Id.Equals(id.Value));

            await Task.Run(() =>
            {
                if (profile != null)
                {
                    _dbContext.Users.Remove(profile.Account);
                    _dbContext.Profiles.Remove(profile);
                    _dbContext.SaveChanges();
                }
            });

            return RedirectToAction("Index");
        }

        public async Task ServerSideValidation(Profile obj, string roleName, int tariffId)
        {
            TryValidateModel(obj);
            ModelState.Remove("Tariff.Name");


            var isUserNameExist = await _dbContext.Users
                .AnyAsync(u => u.UserName.Equals(obj.Account.UserName));
            var isEmailExist = await _dbContext.Users
                    .AnyAsync(u => u.Email.Equals(obj.Account.Email));
            if (ActionName.Equals(nameof(Create)))
            {
                if (isUserNameExist)
                { ModelState.AddModelError("Account.UserName", "Введённый логин уже существует, выберите другой"); }
                if (isUserNameExist)
                { ModelState.AddModelError("Account.Email", "Введённый почтовый адрес уже сущесвует, выберите другой"); }
            }
            else
            {
                var accExisting = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id.Equals(obj.Account.Id));
                if (accExisting != null)
                {
                    if (!accExisting.UserName.Equals(obj.Account.UserName) && isUserNameExist)
                    { ModelState.AddModelError("Account.UserName", "Введённый логин уже существует, выберите другой"); }
                    if (!accExisting.Email.Equals(obj.Account.Email) && isEmailExist)
                    { ModelState.AddModelError("Account.Email", "Введённый почтовый адрес уже сущесвует, выберите другой"); }
                }
                else
                { ModelState.AddModelError("", "Аккаунт профиля не найден"); }
            }

            var pattern = @"^(?=.*[a-z0-9])[a-z][a-z\d.-]{0,19}$";
            var regexCheck = Regex.IsMatch(obj.Account.UserName, pattern);
            if (!regexCheck)
            { ModelState.AddModelError("Account.UserName",
                "Логин должен соответствовать латинским маленьким буквам и быть не длинее 19 символов"); }

            var isTariffExist = await _dbContext.Tariffs
                .AnyAsync(t => t.Id.Equals(tariffId));
            if (!isTariffExist)
            { ModelState.AddModelError("Tariff", "Выбранного тарифа не существует"); }

            var isRoleExist = await _dbContext.Roles
                .AnyAsync(r => r.Name.Equals(roleName));
            if(!isRoleExist)
            { ModelState.AddModelError("", "Выбранной роли не существует"); }
        }
    }
}
