using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Data;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Controllers
{
    [DisplayName("Роли")]
    public class RolesController : CustomController
    {
        public RolesController(BillingDbContext dbContext, RoleManager<Role> roleManager, UserManager<IdentityUser> userManager, IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, userManager, scopeFactory)
        {
        }
    }
}
