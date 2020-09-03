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

namespace EasyBilling.Controllers
{
    [NoShowToMenu]
    [DisplayName("Клиент")]
    public class ClientController : CustomController
    {
        public ClientController(BillingDbContext dbContext,
            RoleManager<Models.Pocos.Role> roleManager,
            IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, scopeFactory)
        { }

        [HttpGet]
        public override async Task<IActionResult> Index(
            string sort = "Id",
            SortType sortType = SortType.ASC,
            int page = 1,
            int pageSize = 10,
            string search = "")
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
                    controllerName: ControllerName,
                    filter: filter,
                    includeFields: new string[]
                    {
                        nameof(Payment.DestinationProfile),
                        nameof(Payment.SourceProfile)
                    },
                    sortType: sortType,
                    sortField: sort,
                    page: page,
                    pageSize: pageSize,
                    searchRequest: search
                );

                return View("CustomIndex", model: ( dvm, profile ));
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChnageHoldState(bool state)
        {
            var profile = await _dbContext.Profiles
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.Account.UserName.Equals(User.Identity.Name));

            return await Task.Run(() =>
            {
                if (profile != null)
                {
                    profile.IsHolded = state;

                    _dbContext.Update(profile);
                    _dbContext.SaveChanges();
                }

                return Redirect("Index");
            });
        }
    }
}
