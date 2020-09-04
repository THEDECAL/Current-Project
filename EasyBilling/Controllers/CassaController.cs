using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models;
using EasyBilling.Models.Pocos;
using EasyBilling.Services;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Controllers
{
    [DisplayName("Касса")]
    public class CassaController : CustomController
    {
        private AccessRightsManager _rightsManager;

        public CassaController(BillingDbContext dbContext,
            RoleManager<Models.Pocos.Role> roleManager,
            AccessRightsManager rightsManager,
            IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, scopeFactory)
        {
            _rightsManager = rightsManager;
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


        [DisplayName(("Оплата"))]
        [HttpGet]
        public async Task<IActionResult> AddUpdateForm(int? id = null)
        {
            return await Task.Run(async () =>
            {
                var model = new Payment();
                if (id != null)
                {
                    model.DestinationProfile = await _dbContext.Profiles
                    .FirstOrDefaultAsync(p => p.Id.Equals(id.Value));
                }

                return View(nameof(AddUpdateForm), model: model);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment obj, int? dstId, string sum)
        {
            try
            {
               obj.Amount = Double.Parse(sum.Replace('.', ','));
            }
            catch (Exception)
            { }

            obj.DestinationProfile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id.Equals(dstId));
            obj.SourceProfile = await _dbContext.Profiles
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.Account.UserName.Equals(User.Identity.Name));
            obj.Role = await _rightsManager.GetRoleAsync(User.Identity.Name);

            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                await Task.Run(() =>
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        obj.DestinationProfile.AmountOfCash += obj.Amount;

                        _dbContext.Update(obj.DestinationProfile);
                        _dbContext.Add(obj);
                        _dbContext.SaveChanges();

                        transaction.Commit();
                    }
                });

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        public async Task ServerSideValidation(Payment obj)
        {
            if (obj.Amount <= 0)
            { ModelState.AddModelError("", "Сумма должна быть больше 0"); }

            if (obj.DestinationProfile == null || obj.SourceProfile == null)
            { ModelState.AddModelError("", "Получатель или отправитель оплаты отсутствует"); }

            if (obj.Role == null)
            { ModelState.AddModelError("", "Не известна ваша роль"); }
        }
    }
}
