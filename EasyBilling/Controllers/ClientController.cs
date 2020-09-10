using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models.Pocos;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Threading.Tasks;
using System;
using EasyBilling.Models;
using EasyBilling.Services;

namespace EasyBilling.Controllers
{
    [NoShowInMenu]
    [DisplayName("Клиент")]
    public class ClientController : CustomController
    {
        private TariffRegulator _tariffRegulator;
        public ClientController(BillingDbContext dbContext,
            RoleManager<Role> roleManager,
            UserManager<IdentityUser> userManager,
            TariffRegulator tariffRegulator,
            IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, userManager, scopeFactory)
        {
            _tariffRegulator = tariffRegulator;
        }

        [DisplayName("Абонетский кабинет")]
        [HttpGet]
        public override async Task<IActionResult> Index()
        {
            var profile = await _dbContext.Profiles
                .Include(p => p.Account)
                .Include(p => p.Tariff)
                .FirstOrDefaultAsync(p => p.Account.UserName.Equals(User.Identity.Name));
            var filter = new Func<Payment, bool>((o)
                => o.DestinationProfile.Id.Equals(profile.Id));

            return await Task.Run(() =>
            {
                var dvm = new DataViewModel<Payment>(_scopeFactory,
                    settings: Settings,
                    urlPath: HttpContext.Request.Path,
                    filter: filter,
                    includeFields: new string[]
                    {
                        nameof(Payment.SourceProfile),
                        nameof(Payment.DestinationProfile)
                    },
                    excludeFields: new string[]
                    {
                        nameof(Payment.DestinationProfile)
                    }
                );

                return View("CustomIndex", model: (dvm, profile));
            });
        }

        [DisplayName("Абонетский кабинет по ID")]
        [HttpGet]
        public async Task<IActionResult> Get(int id)
            => await Task.Run(async () => 
            {
            var profile = await _dbContext.Profiles
                .Include(p => p.Account)
                .Include(p => p.Tariff)
                .FirstOrDefaultAsync(p => p.Id.Equals(id));

            if (profile == null)
                return View("Index");

            var filter = new Func<Payment, bool>((o)
                => o.DestinationProfile.Id.Equals(profile.Id));

                var dvm = new DataViewModel<Payment>(_scopeFactory,
                    settings: Settings,
                    urlPath: HttpContext.Request.Path,
                    filter: filter,
                    includeFields: new string[]
                    {
                        nameof(Payment.SourceProfile),
                        nameof(Payment.DestinationProfile)
                    },
                    excludeFields: new string[]
                    {
                        nameof(Payment.DestinationProfile)
                    }
                );

                return View("CustomIndex", model: (dvm, profile));
            });

        [DisplayName("Смена состояния заморозки")]
        [HttpPost]
        public async Task<IActionResult> ChnageHoldState()
        {
            var profile = await _dbContext.Profiles
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.Account.UserName.Equals(User.Identity.Name));

            return await Task.Run(() =>
            {
                if (profile != null)
                {
                    profile.IsHolded = !profile.IsHolded;

                    _dbContext.Update(profile);
                    _dbContext.SaveChanges();
                    _tariffRegulator.StartToUseOfTariffAsync(profile.Id).Wait();
                }

                return Redirect("Index");
            });
        }

        [DisplayName("Смена тарифа")]
        [HttpPost]
        public async Task<IActionResult> ChangeTariff(int? tariffId = null)
        {
            return await Task.Run(async () =>
            {
                var tariff = await _dbContext.Tariffs
                    .FirstOrDefaultAsync(t => t.Id.Equals(tariffId));

                if (tariff != null)
                {
                    var profile = await _dbContext.Profiles
                        .Include(p => p.Account)
                        .FirstOrDefaultAsync(p => p.Account.UserName.Equals(User.Identity.Name));

                    profile.Tariff = tariff;

                    _dbContext.Update(profile);
                    await _dbContext.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            });
        }
    }
}
