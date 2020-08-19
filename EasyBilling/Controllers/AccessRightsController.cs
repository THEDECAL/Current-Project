using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Helpers;
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
    [Authorize]
    [CheckAccessRights]
    [DisplayName("Права доступа")]
    public class AccessRightsController : CustomController
    {
        private readonly BillingDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IServiceScopeFactory _scopeFactory;
        public AccessRightsController(BillingDbContext dbContext,

            RoleManager<IdentityRole> roleManager,
            IServiceScopeFactory scopeFactory)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _scopeFactory = scopeFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string sort = "Id",
            SortType sortType = SortType.ASC,
            int page = 1,
            int pageSize = 10,
            string search = "")
        {
            return await Task.Run(() =>
            {
                ViewData["Title"] = DisplayName;

                var dvm = new DataViewModel<AccessRight>(_scopeFactory,
                    includeField1: "Role",
                    sortType: sortType,
                    sortField: sort,
                    page: page,
                    pageSize: pageSize,
                    searchRequest: search
                );
                //var roles = _roleManager.Roles.ToList();
                //var cntrlsNames = ControllerHelper.GetControllersNames();
                //var model = new AccessRightsViewModel(dvm, roles, cntrlsNames);

                return View(model: dvm);
            });
        }
        [HttpGet]
        public async Task<ActionResult> AddUpdateForm(int? id = null)
        {
            return await Task.Run(async () =>
            {
                ViewData["Title"] = DisplayName;
                ViewData["ActionPage"] = nameof(Create);

                AccessRight ar = new AccessRight();
                if (id != null)
                {
                    ar = await _dbContext.AccessRights.FindAsync(id);
                    if (ar == null)
                        ar = new AccessRight();
                    else
                        ViewData["ActionPage"] = nameof(Update);
                }

                return View(nameof(AddUpdateForm), model: ar);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccessRight rights)
        {
            if (await ServerSideValidation(rights) && ModelState.IsValid)
            {
                rights.Role = await _roleManager.FindByNameAsync(rights.Role.Name);
                await _dbContext.AccessRights.AddAsync(rights);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        [HttpPost]
        public async Task<IActionResult> Update(AccessRight rights)
        {
            if (await ServerSideValidation(rights) && ModelState.IsValid)
            {
                rights.Role = await _roleManager.FindByNameAsync(rights.Role.Name);
                await Task.Run(() =>
                {
                    _dbContext.AccessRights.Update(rights);
                    _dbContext.SaveChanges();
                });

                return RedirectToAction("Index");
            }

            return await AddUpdateForm(rights.Id);
        }

        private async Task<bool> ServerSideValidation(AccessRight rights)
        {
            var cntrlExist = await ControllerHelper.IsExistAsync(rights.ControllerName);
            if (!cntrlExist)
            { ModelState.AddModelError("ControllerName", "Выбранная страница не существует"); }
            var role = await _roleManager.FindByNameAsync(rights.Role.Name);
            if (role == null)
            { ModelState.AddModelError("Role", "Выбранная роль не существует"); }
            TryValidateModel(rights);

            return (cntrlExist && role != null) ? true : false;
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var ar = await _dbContext.AccessRights.FindAsync(id);
            await Task.Run(() =>
            {
                if (ar != null)
                {
                    _dbContext.AccessRights.Remove(ar);
                    _dbContext.SaveChanges();
                }
            });

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CheckCntrlExist([NotNull] string controllerName)
            => Json(await ControllerHelper.IsExistAsync(controllerName));

        public async Task<IActionResult> CheckRoleExist([NotNull] string role)
        {
            var roleExisting = await _roleManager.FindByNameAsync(role);
            return Json((roleExisting != null)?true:false);
        }
    }
}
