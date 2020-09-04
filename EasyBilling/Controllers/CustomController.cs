using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    [Authorize]
    [CheckAccessRights]
    public abstract class CustomController : Controller
    {
        protected readonly BillingDbContext _dbContext;
        protected readonly RoleManager<EasyBilling.Models.Pocos.Role> _roleManager;
        protected readonly IServiceScopeFactory _scopeFactory;

        public string DisplayName { get => (GetType().GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute).DisplayName; }
        public string ControllerName { get => GetType().Name.Replace("Controller", ""); }
        public string ActionName { get => RouteData.Values["action"] as string; }

        public CustomController(BillingDbContext dbContext,
                RoleManager<EasyBilling.Models.Pocos.Role> roleManager,
                IServiceScopeFactory scopeFactory)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _scopeFactory = scopeFactory;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Index(ControlPanelSettings settings = null)
            => await Task.Run(() => View());

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string actionDisplayName = "";
            try
            {
                var actionDnAtt = ControllerContext.ActionDescriptor.MethodInfo
                    .GetCustomAttributes(typeof(DisplayNameAttribute), false)[0] as DisplayNameAttribute;
                actionDisplayName = " - " + actionDnAtt.DisplayName;
            }
            catch (Exception)
            { }

            ViewData["Title"] = $"{DisplayName}{actionDisplayName}";
            ViewData["ControllerName"] = ControllerName;

            return base.OnActionExecutionAsync(context, next);
        }
    }
}