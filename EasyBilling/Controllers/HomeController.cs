using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using EasyBilling.Attributes;
using EasyBilling.Models;
using System.ComponentModel;
using EasyBilling.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Controllers
{
    [NoShowToMenu]
    [DisplayName("Главная")]
    public class HomeController : CustomController
    {
        public HomeController(BillingDbContext dbContext,
            RoleManager<Models.Pocos.Role> roleManager,
            IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, scopeFactory)
        { }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
        }

        public IActionResult ErrorAccess()
        {
            ViewData["Title"] = "Отказано в доступе";
            return View();
        }
    }
}
