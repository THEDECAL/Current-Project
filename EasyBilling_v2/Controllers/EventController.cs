using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Controllers
{
    [NoShowInMenu]
    [DisplayName("Журнал событий")]
    public class EventController : ExtController<EventLog>
    {
        public EventController(BillingDbContext dbContext, RoleManager<Role> roleManager, UserManager<User> userManager, IServiceScopeFactory scopeFactory) : base(dbContext, roleManager, userManager, scopeFactory)
        {
        }
    }
}
