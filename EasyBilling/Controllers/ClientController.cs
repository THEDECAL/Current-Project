using EasyBilling.Attributes;
using EasyBilling.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace EasyBilling.Controllers
{
    [NoShowToMenu]
    [DisplayName("Клиент")]
    public class ClientController : CustomController
    {
        public ClientController(BillingDbContext dbContext,
            RoleManager<IdentityRole> roleManager,
            IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, scopeFactory)
        { }
    }
}
