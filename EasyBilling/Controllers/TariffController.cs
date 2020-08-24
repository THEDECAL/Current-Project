using System;
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
    [DisplayName("Тарифы")]
    public class TariffController : CustomController, ICustomControllerCrud<Tariff>
    {
        public TariffController(BillingDbContext dbContext,
         RoleManager<Models.Pocos.Role> roleManager,
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
                var dvm = new DataViewModel<Tariff>(_scopeFactory,
                    controllerName: ControllerName,
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

                var model = new Tariff();
                if (id != null)
                {
                    model = await _dbContext.Tariffs.FindAsync(id);
                    if (model == null)
                        model = new Tariff();
                    else
                        ViewData["ActionPage"] = nameof(Update);
                }

                return View(nameof(AddUpdateForm), model: model);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tariff obj)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                await _dbContext.Tariffs.AddAsync(obj);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        [HttpPost]
        public async Task<IActionResult> Update(Tariff obj)
        {
            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                await Task.Run(() =>
                {
                    _dbContext.Tariffs.Update(obj);
                    _dbContext.SaveChanges();
                });

                return RedirectToAction("Index");
            }

            return await AddUpdateForm(obj.Id);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id = null)
        {
            var obj = await _dbContext.Tariffs.FindAsync(id);
            await Task.Run(() =>
            {
                if (obj != null)
                {
                    _dbContext.Tariffs.Remove(obj);
                    _dbContext.SaveChanges();
                }
            });

            return RedirectToAction("Index");
        }

        public async Task ServerSideValidation(Tariff obj)
        {
            TryValidateModel(obj);
            if (!ActionName.Equals(nameof(Update))) {
                var tariffExist = await _dbContext.Tariffs.AnyAsync(t => t.Name.Equals(obj.Name));
                if (tariffExist)
                { ModelState.AddModelError("Name", "Такое название тарифа уже существует, выберите другое"); }
            }
        }
    }
}
