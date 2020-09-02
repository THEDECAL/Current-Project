﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Controllers
{
    [DisplayName("Устройства")]
    public class DeviceController : CustomController, ICustomControllerCrud<Device>
    {
        public DeviceController(BillingDbContext dbContext,
            RoleManager<Models.Pocos.Role> roleManager,
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
                var dvm = new DataViewModel<Device>(_scopeFactory,
                    controllerName: ViewData["ControllerName"] as string,
                    includeFields: new string[]
                    {
                        nameof(Device.Type),
                        nameof(Device.State)
                    },
                    excludeFields: new string[]
                    {
                        nameof(Device.CustomDeviceField1),
                        nameof(Device.CustomDeviceField2),
                        nameof(Device.CustomDeviceField3),
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

                var model = new Device();
                if (id != null)
                {
                    model = await _dbContext.Devices
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Device obj)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                obj.State = await _dbContext.DeviceStates
                    .FirstOrDefaultAsync(o => o.Name.Equals(obj.State.Name));
                obj.Type = await _dbContext.DeviceTypes
                    .FirstOrDefaultAsync(o => o.Name.Equals(obj.Type.Name));

                await _dbContext.Devices.AddAsync(obj);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Device obj)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                obj.State = await _dbContext.DeviceStates
                    .FirstOrDefaultAsync(s => s.Name.Equals(obj.State.Name));
                obj.Type = await _dbContext.DeviceTypes
                    .FirstOrDefaultAsync(s => s.Name.Equals(obj.Type.Name));

                await Task.Run(() =>
                {
                    _dbContext.Devices.Update(obj);
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

        public async Task ServerSideValidation(Device obj)
        {
            TryValidateModel(obj);

            var checkType = await _dbContext.DeviceTypes
                .AnyAsync(t => t.Name.Equals(obj.Type.Name));
            if (!checkType)
            { ModelState.AddModelError("Type", "Выбранный тип устройства не существует"); }

            var checkState = await _dbContext.DeviceStates
                .AnyAsync(t => t.Name.Equals(obj.State.Name));
            if (!checkState)
            { ModelState.AddModelError("State", "Выбранное состояние устройства не существует"); }
        }

        public async Task<IActionResult> CheckType([NotNull] string typeName)
            => Json(await _dbContext.DeviceTypes
                .AnyAsync(t => t.Name.Equals(typeName)));

        public async Task<IActionResult> CheckState([NotNull] string stateName)
            => Json(await _dbContext.DeviceStates
                .FirstOrDefaultAsync(t => t.Name.Equals(stateName)));
    }
}
