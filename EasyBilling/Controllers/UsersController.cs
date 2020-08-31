using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models.Pocos;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        { }

        [HttpGet]
        [DisplayName("Список")]
        public async override Task<IActionResult> Index(
            string sort = "Id",
            SortType sortType = SortType.ASC,
            int page = 1,
            int pageSize = 10,
            string search = "")
        {
            return await Task.Run(() =>
            {
                var dvm = new DataViewModel<Profile>(_scopeFactory,
                    controllerName: ViewData["ControllerName"] as string,
                    includeFields: new string[]
                    {
                        nameof(Profile.Tarrif),
                        nameof(Profile.Account)
                    },
                    excludeFields: new string[]
                    {
                        nameof(Profile.Patronymic),
                        nameof(Profile.Comment),
                        nameof(Profile.AmountOfCash),
                        nameof(Profile.LastLogin),
                        nameof(Profile.CustomProfileField1),
                        nameof(Profile.CustomProfileField2),
                        nameof(Profile.CustomProfileField3)
                    },
                    sortType: sortType,
                    sortField: sort,
                    page: page,
                    pageSize: pageSize,
                    searchRequest: search
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
        public async Task<IActionResult> Create(Profile obj, string roleId)
        {
            await ServerSideValidation(obj, roleId);
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(obj.Account);
                if (result.Succeeded)
                {
                    var role = await _roleManager.FindByIdAsync(roleId);
                    await _userManager.AddToRoleAsync(obj.Account, role.Name);
                }

                await _dbContext.Profiles.AddAsync(obj);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        [HttpPost]
        public async Task<IActionResult> Update(Profile obj, string roleId)
        {
            await ServerSideValidation(obj, roleId);
            if (ModelState.IsValid)
            {
                obj.Account = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Id.Equals(obj.Account.Id));
                var role = await _roleManager.FindByIdAsync(roleId);
                var oldRoles = await _dbContext.UserRoles
                    .Where(u => u.UserId.Equals(obj.Account.Id)).ToArrayAsync();
                _dbContext.UserRoles.RemoveRange(oldRoles);
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
        public async Task<IActionResult> Delete(int? id = null)
        {
            var obj = await _dbContext.Devices.FindAsync(id);
            await Task.Run(() =>
            {
                if (obj != null)
                {
                    _dbContext.Devices.Remove(obj);
                    _dbContext.SaveChanges();
                }
            });

            return RedirectToAction("Index");
        }

        public async Task ServerSideValidation(Profile obj, string roleId)
        {
            TryValidateModel(obj);

            var isRoleExist = await _dbContext.Roles
                .AnyAsync(r => r.Id.Equals(roleId));
            if(!isRoleExist)
            { ModelState.AddModelError("", "Такой роли не существует"); }

            var isAccLoginExist = await _dbContext.Users
                .AnyAsync(u => u.UserName.Equals(obj.Account.UserName));
            if (isAccLoginExist)
            { ModelState.AddModelError("Account.UserName", "Такой логин уже существует"); }

            var isAccEmailExist = await _dbContext.Users
                .AnyAsync(u => u.Email.Equals(obj.Account.Email));
            if (isAccEmailExist)
            { ModelState.AddModelError("Account.Email", "Такой электронный адрес уже зарегестрирован"); }
        }
    }
}
