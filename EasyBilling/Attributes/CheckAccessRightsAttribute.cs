using EasyBilling.Data;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace EasyBilling.Attributes
{
    public class CheckAccessRightsAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private BillingDbContext _dbContext;
        private UserManager<IdentityAccount> _um;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _dbContext = context.HttpContext.RequestServices
                .GetRequiredService<BillingDbContext>();
            _um = context.HttpContext.RequestServices
                .GetRequiredService<UserManager<IdentityAccount>>();

            //Приведение к типу для получения функций контроллера и действий
            var ad = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)
                context.ActionDescriptor;
            var controller = ad.ControllerName;
            //var userRoleId = context.HttpContext.User
            //    .FindFirstValue(ClaimTypes.Role);

            var user = _um.FindByNameAsync(context.HttpContext.User.Identity.Name).Result;
            var roles = _um.GetRolesAsync(user).Result;

            if (roles != null && roles.Count != 0)
            {
                AccessRight accessRights = null;

                using (_dbContext)
                {
                    accessRights = _dbContext.AccessRights
                        .Include(ar => ar.Role)
                        .FirstOrDefault(ar =>
                            ar.Role.Name.Equals(roles[0]) &&
                            ar.ControllerName.Equals(controller));
                }
                if (accessRights != null && accessRights.IsAvailable)
                    return;
            }
            context.HttpContext.Response.Redirect($"/Home/ErrorAccess/{ad.DisplayName}");

            //throw new UnauthorizedAccessException("Отказано в доступе.");
        }
    }
}