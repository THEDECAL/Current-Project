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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Controllers
{
    [DisplayName("Финансовые операции")]
    public class FinancialOperationsController : CustomController
    {
        public FinancialOperationsController(BillingDbContext dbContext,
            RoleManager<Models.Pocos.Role> roleManager,
            IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, scopeFactory)
        { }

        [HttpGet]
        [DisplayName("Список")]
        public override async Task<IActionResult> Index(ControlPanelSettings settings = null)
        {
            return await Task.Run(() =>
            {
                var dvm = new DataViewModel<Payment>(_scopeFactory,
                    controllerName: ViewData["ControllerName"] as string,
                    settings: settings,
                    includeFields: new string[]
                    {
                        nameof(Payment.SourceProfile),
                        nameof(Payment.DestinationProfile),
                        nameof(Payment.Role),
                        "SourceProfile.Account",
                        "DestinationProfile.Account"
                    }
                );

                return View("CustomIndex", model: dvm);
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id = null)
        {
            var obj = await _dbContext.Payments
                .Include(p => p.DestinationProfile)
                .FirstOrDefaultAsync(p => p.Id.Equals(id));

            if (obj != null)
            {
                await Task.Run(() =>
                {
                    using (var transaction = _dbContext.Database.BeginTransaction())
                    {
                        if (obj.Amount > 0)
                        { obj.DestinationProfile.AmountOfCash -= obj.Amount; }
                        else
                        { obj.DestinationProfile.AmountOfCash += obj.Amount; }

                        _dbContext.Update(obj.DestinationProfile);
                        _dbContext.Payments.Remove(obj);
                        _dbContext.SaveChanges();

                        transaction.Commit();
                    }
                });
            }

            return RedirectToAction("Index");
        }
    }
}
