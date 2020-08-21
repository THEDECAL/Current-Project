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
    [DisplayName("Права доступа")]
    public class AccessRightsController : CustomController, ICustomControllerCrud<AccessRight>
    {
        public AccessRightsController(BillingDbContext dbContext,
            RoleManager<IdentityRole> roleManager,
            IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, scopeFactory)
        { }
        [HttpGet]
        [DisplayName("Список")]
        public override async Task<IActionResult> Index(
            string sort = "Id",
            SortType sortType = SortType.ASC,
            int page = 1,
            int pageSize = 10,
            string search = "")
        {
            return await Task.Run(() =>
            {
                var dvm = new DataViewModel<AccessRight>(_scopeFactory,
                    controllerName: ViewData["ControllerName"] as string,
                    includeField1: "Role",
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

                var model = new AccessRight();
                if (id != null)
                {
                    model = await _dbContext.AccessRights.FindAsync(id);
                    if (model == null)
                        model = new AccessRight();
                    else
                        ViewData["ActionPage"] = nameof(Update);
                }

                return View(nameof(AddUpdateForm), model: model);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccessRight obj)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                obj.Role = await _roleManager.FindByNameAsync(obj.Role.Name);
                await _dbContext.AccessRights.AddAsync(obj);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        [HttpPost]
        public async Task<IActionResult> Update(AccessRight obj)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                obj.Role = await _roleManager.FindByNameAsync(obj.Role.Name);
                await Task.Run(() =>
                {
                    _dbContext.AccessRights.Update(obj);
                    _dbContext.SaveChanges();
                });

                return RedirectToAction("Index");
            }

            return await AddUpdateForm(obj.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var obj = await _dbContext.AccessRights.FindAsync(id);
            await Task.Run(() =>
            {
                if (obj != null)
                {
                    _dbContext.AccessRights.Remove(obj);
                    _dbContext.SaveChanges();
                }
            });

            return RedirectToAction("Index");
        }

        public async Task ServerSideValidation(AccessRight obj)
        {
            TryValidateModel(obj);
            var cntrlExist = await ControllerHelper.IsExistAsync(obj.ControllerName);
            if (!cntrlExist)
            { ModelState.AddModelError("ControllerName", "Выбранная страница не существует"); }
            var role = await _roleManager.FindByNameAsync(obj.Role.Name);
            if (role == null)
            { ModelState.AddModelError("Role", "Выбранная роль не существует"); }
            var accessRightExisting = _dbContext.AccessRights.Include("Role")
                .FirstOrDefault(ar => ar.Role.Name.Equals(obj.Role.Name) &&
                ar.ControllerName.Equals(obj.ControllerName));
            if (accessRightExisting != null)
            { ModelState.AddModelError("", "Правило для это роли и страницы уже есть, измените его"); }
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
