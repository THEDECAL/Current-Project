using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models;
using EasyBilling.Models.Entities;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Controllers
{
    [DisplayName("Устройства")]
    public class DeviceController : ExtController<Device>
    {
        public DeviceController(BillingDbContext dbContext, RoleManager<Role> roleManager, UserManager<User> userManager, IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, userManager, scopeFactory)
        {
        }

        //[HttpGet]
        //[DisplayName("Список")]
        //public async override Task<IActionResult> Index()
        //{
        //    return await Task.Run(() =>
        //    {
        //        //var dvm = new TableDataViewModel<Device>(_ssFactory,
        //        //    state: PagState,
        //        //    urlPath: HttpContext.Request.Path,
        //        //    includeFields: new string[]
        //        //    {
        //        //        nameof(Device.Type),
        //        //        nameof(Device.State)
        //        //    },
        //        //    excludeFields: new string[]
        //        //    {
        //        //        nameof(Device.CustomDeviceField1),
        //        //        nameof(Device.CustomDeviceField2),
        //        //        nameof(Device.CustomDeviceField3),
        //        //        nameof(Device.DateOfCreation),
        //        //        nameof(Device.DateOfUpdate),
        //        //    }
        //        //);

        //        return View("CustomIndex", model: dvm);
        //    });
        //}

        [DisplayName(("Форма добавить/изменить"))]
        [HttpGet]
        public async Task<IActionResult> AddUpdateForm(int? id = null)
        {
            return await Task.Run(async () =>
            {
                ViewData["ActionPage"] = nameof(Create);

                var model = new Device();
                if (id != null)
                {
                    model = await DbContext.Devices
                        .Include(d => d.Type)
                        .Include(d => d.State)
                        .FirstOrDefaultAsync(d => d.Id.Equals(id));
                    if (model == null)
                        model = new Device();
                    else
                        ViewData["ActionPage"] = nameof(Update);
                }

                return View(nameof(AddUpdateForm), model: model);
            });
        }

        [DisplayName("Создать")]
        [HttpPost]
        public async Task<IActionResult> Create(Device obj)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                obj.State = await DbContext.DeviceStates
                    .FirstOrDefaultAsync(o => o.Name.Equals(obj.State.Name));
                obj.Type = await DbContext.DeviceTypes
                    .FirstOrDefaultAsync(o => o.Name.Equals(obj.Type.Name));

                await DbContext.Devices.AddAsync(obj);
                await DbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        [DisplayName("Изменить")]
        [HttpPost]
        public async Task<IActionResult> Update(Device obj)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                obj.State = await DbContext.DeviceStates
                    .FirstOrDefaultAsync(s => s.Name.Equals(obj.State.Name));
                obj.Type = await DbContext.DeviceTypes
                    .FirstOrDefaultAsync(s => s.Name.Equals(obj.Type.Name));

                await Task.Run(() =>
                {
                    DbContext.Devices.Update(obj);
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
            var obj = await DbContext.Devices.FindAsync(id);
            await Task.Run(() =>
            {
                if (obj != null)
                {
                    DbContext.Devices.Remove(obj);
                    DbContext.SaveChanges();
                }
            });

            return RedirectToAction("Index");
        }

        public async Task ServerSideValidation(Device obj)
        {
            TryValidateModel(obj);

            var checkType = await DbContext.DeviceTypes
                .AnyAsync(t => t.Name.Equals(obj.Type.Name));
            if (!checkType)
            { ModelState.AddModelError("Type", "Выбранный тип устройства не существует"); }

            var checkState = await DbContext.DeviceStates
                .AnyAsync(t => t.Name.Equals(obj.State.Name));
            if (!checkState)
            { ModelState.AddModelError("State", "Выбранное состояние устройства не существует"); }
        }

        public async Task<IActionResult> CheckType([NotNull] string typeName)
            => Json(await DbContext.DeviceTypes
                .AnyAsync(t => t.Name.Equals(typeName)));

        public async Task<IActionResult> CheckState([NotNull] string stateName)
            => Json(await DbContext.DeviceStates
                .FirstOrDefaultAsync(t => t.Name.Equals(stateName)));
    }
}
