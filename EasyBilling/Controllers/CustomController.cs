using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    [Authorize]
    [CheckAccessRights]
    public abstract class CustomController : Controller
    {
        protected readonly BillingDbContext _dbContext;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly IServiceScopeFactory _scopeFactory;

        public string DisplayName { get; }
        public CustomController(BillingDbContext dbContext,
                RoleManager<IdentityRole> roleManager,
                IServiceScopeFactory scopeFactory)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _scopeFactory = scopeFactory;

            DisplayName = (GetType()
                .GetCustomAttributes(typeof(DisplayNameAttribute), true)
                .SingleOrDefault() as DisplayNameAttribute).DisplayName;
        }
        [HttpGet]
        public virtual async Task<IActionResult> Index(
            string sort = "Id",
            SortType sortType = SortType.ASC,
            int page = 1,
            int pageSize = 10,
            string search = "") => await Task.Run(() => View());

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
            ViewData["ControllerName"] = GetType().Name.Replace("Controller", "");
            return base.OnActionExecutionAsync(context, next);
        }
    }
}