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
using EasyBilling.Models.Entities;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Role = EasyBilling.Models.Entities.Role;

namespace EasyBilling.Controllers
{
    [DisplayName("Права доступа")]
    public class AccessRightsController : ExtController<AccessRight>
    {
        public AccessRightsController(BillingDbContext ctx,
            RoleManager<Role> roleMngr,
            UserManager<User> userMngr, 
            IServiceScopeFactory ssFactory)
            : base(ctx, roleMngr, userMngr, ssFactory) { }

        //[HttpGet]
        //[DisplayName("Список")]
        //public override async Task<IActionResult> Index() => await base.Index();

        [DisplayName(("Форма добавить/изменить"))]
        [HttpGet]
        public async Task<IActionResult> AddUpdateForm(int? id = null)
        {
            return await Task.Run(async () =>
            {
                ViewData["ActionPage"] = nameof(Add);

                var model = new AccessRight();
                if (id != null)
                {
                    model = await DbContext.AccessRights
                        .Include(ar => ar.Role)
                        .Include(ar => ar.Controller)
                        .FirstOrDefaultAsync(ar => ar.Id.Equals(id));
                    if (model == null)
                        model = new AccessRight();
                    else
                        ViewData["ActionPage"] = nameof(Change);
                }

                return View(nameof(AddUpdateForm), model: model);
            });
        }

        [DisplayName(("Создать"))]
        [HttpPost]
        public async Task<IActionResult> Create(AccessRight obj, List<bool> rights)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                obj.RoleId = (await RoleMngr.FindByNameAsync(obj.Role.Name)).Id;
                obj.Role = null;
                obj.Controller = await DbContext.ControllersNames
                    .FirstOrDefaultAsync(c => c.Name.Equals(obj.Controller.Name));
                obj.UpdateActionsRights(rights);
                await DbContext.AccessRights.AddAsync(obj);
                await DbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        [DisplayName(("Изменить"))]
        [HttpPost]
        public async Task<IActionResult> Update(AccessRight obj, List<bool> rights)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                obj.RoleId = (await RoleMngr.FindByNameAsync(obj.Role.Name)).Id;
                obj.Role = null;
                obj.Controller = await DbContext.ControllersNames
                    .FirstOrDefaultAsync(c => c.Name.Equals(obj.Controller.Name));
                obj.UpdateActionsRights(rights);
                await Task.Run(() =>
                {
                    DbContext.AccessRights.Update(obj);
                    DbContext.SaveChanges();
                });

                return RedirectToAction("Index");
            }

            return await AddUpdateForm(obj.Id);
        }

        [DisplayName(("Удалить"))]
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var obj = await DbContext.AccessRights.FindAsync(id);
            await Task.Run(() =>
            {
                if (obj != null)
                {
                    DbContext.AccessRights.Remove(obj);
                    DbContext.SaveChanges();
                }
            });

            return RedirectToAction("Index");
        }

        public async Task ServerSideValidation(AccessRight obj)
        {
            TryValidateModel(obj);
            ModelState.Remove("Role.LocalizedName");
            ModelState.Remove("Role.DefaultControllerName.Name");
            var cntrlExist = await DbContext.ControllersNames
                .AnyAsync(c => c.Name.Equals(obj.Controller.Name));
            if (!cntrlExist)
            { ModelState.AddModelError("ControllerName", "Выбранная страница не существует"); }
            var role = await RoleMngr.FindByNameAsync(obj.Role.Name);
            if (role == null)
            { ModelState.AddModelError("Role", "Выбранная роль не существует"); }
            if (!ActionName.Equals(nameof(Change)))
            {
                var accessRightExisting = DbContext.AccessRights.Include("Role")
                    .FirstOrDefault(ar => ar.Role.Name.Equals(obj.Role.Name) &&
                    ar.Controller.Name.Equals(obj.Controller.Name));
                if (accessRightExisting != null)
                { ModelState.AddModelError("", "Правило для этой роли и страницы уже есть, измените его"); }
            }
        }

        public async Task<IActionResult> CheckCntrlExist([NotNull] string controllerName)
            => Json(await DbContext.ControllersNames
                .FirstOrDefaultAsync(c => c.Name.Equals(controllerName)));

        public async Task<IActionResult> CheckRoleExist([NotNull] string role)
        {
            var roleExisting = await RoleMngr.FindByNameAsync(role);
            return Json((roleExisting != null)?true:false);
        }
    }
}
