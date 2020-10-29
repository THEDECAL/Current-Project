using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models;
using EasyBilling.Models.Entities;
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
    public class CassaController : ExtController<Profile>
    {
        AccessRightsManager _rightsMngr;
        public CassaController(BillingDbContext dbContext,
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            AccessRightsManager rightsMngr,
            IServiceScopeFactory scopeFactory)
            : base(dbContext, roleManager, userManager, scopeFactory)
        { _rightsMngr = rightsMngr; }

        //[HttpGet]
        //[DisplayName("Список")]
        //public override async Task<IActionResult> Index()
        //{
        //    return await Task.Run(() =>
        //    {
        //        //var dvm = new TableDataViewModel<Profile>(_scopeFactory,
        //        //    settings: Settings,
        //        //    urlPath: HttpContext.Request.Path,
        //        //    includeFields: new string[]
        //        //    {
        //        //        nameof(Profile.Tariff),
        //        //        nameof(Profile.User)
        //        //    },
        //        //    excludeFields: new string[]
        //        //    {
        //        //        nameof(Profile.Patronymic),
        //        //        nameof(Profile.Comment),
        //        //        nameof(Profile.LastOfLogin),
        //        //        nameof(Profile.DateOfUpdate),
        //        //        nameof(Profile.StartUsedOfTariff),
        //        //        nameof(Profile.TrafficUsed),
        //        //        nameof(Profile.CustomProfileField1),
        //        //        nameof(Profile.CustomProfileField2),
        //        //        nameof(Profile.CustomProfileField3),
        //        //        nameof(Profile.DateOfCreation)
        //        //    }
        //        //);

        //        return View("CustomIndex");
        //    });
        //}


        [DisplayName(("Оплата"))]
        [HttpGet]
        public async Task<IActionResult> AddUpdateForm(int? id = null)
        {
            return await Task.Run(async () =>
            {
                var model = new Payment();
                if (id != null)
                {
                    model.DestinationProfile = await DbContext.Profiles
                    .FirstOrDefaultAsync(p => p.Id.Equals(id.Value));
                }

                return View(nameof(AddUpdateForm), model: model);
            });
        }

        [DisplayName("Создать")]
        [HttpPost]
        public async Task<IActionResult> Create(Payment obj, int? dstId, string sum)
        {
            try
            {
                obj.Amount = Double.Parse(sum.Replace('.', ','));
            }
            catch (Exception)
            { }

            obj.DestinationProfile = await DbContext.Profiles
                .FirstOrDefaultAsync(p => p.Id.Equals(dstId));
            obj.SourceProfileId = (await DbContext.Profiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.User.UserName.Equals(User.Identity.Name))).Id;
            obj.RoleId = (await _rightsMngr.GetRoleAsync(User.Identity.Name)).Id;

            await ServerSideValidation(obj);
            if (ModelState.IsValid)
            {
                await Task.Run(() =>
                {
                    using (var transaction = DbContext.Database.BeginTransaction())
                    {
                        obj.DestinationProfile.BalanceOfCash += obj.Amount;

                        DbContext.Profiles.Update(obj.DestinationProfile);
                        DbContext.Payments.Add(obj);
                        DbContext.SaveChanges();

                        transaction.Commit();
                    }
                });

                return RedirectToAction("Index");
            }

            return await AddUpdateForm();
        }

        public async Task ServerSideValidation(Payment obj)
            => await Task.Run(() =>
            {
                if (obj.Amount <= 0)
                { ModelState.AddModelError("", "Сумма должна быть больше 0"); }

                if (obj.DestinationProfile == null || obj.SourceProfileId == 0)
                { ModelState.AddModelError("", "Получатель или отправитель оплаты отсутствует"); }

                //if (string.IsNullOrWhiteSpace(obj.RoleId))
                //{ ModelState.AddModelError("", "Не известна ваша роль"); }
            });
    }
}
