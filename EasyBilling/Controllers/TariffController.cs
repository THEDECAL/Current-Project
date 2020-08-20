using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Controllers
{
    [DisplayName("Тарифы")]
    public class TariffController : CustomController/*, ICustomControllerCrud<Tariff>*/
    {
        public TariffController(BillingDbContext dbContext,
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
                var dvm = new DataViewModel<Tariff>(_scopeFactory,
                    controllerName: ViewData["ControllerName"] as string,
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

        public async Task<IActionResult> Create(Tariff obj)
        {
            return await Task.Run(() => View());
        }

        public async Task<IActionResult> Update(Tariff obj)
        {
            return View();
        }

        //public async Task<IActionResult> Delete(int? id = null)
        //{

        //}

        //public async Task<bool> ServerSideValidation(Tariff obj)
        //{

        //}
    }
}
