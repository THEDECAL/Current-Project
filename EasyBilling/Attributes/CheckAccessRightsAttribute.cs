using EasyBilling.Data;
using EasyBilling.Models;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.ProjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyBilling.Attributes
{
    public class CheckAccessRightsAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private BillingDbContext _dbContext;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //Получаем сервис для работы с БД
            _dbContext = context.HttpContext.RequestServices
                .GetRequiredService<BillingDbContext>();

            //Приведение к типу для получения функций контроллера и действий
            var ad = (Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)
                context.ActionDescriptor;
            var controller = ad.ControllerName;
            //var component = ad.FilterDescriptors.
            var action = ad.ActionName;
            var userRoleId = context.HttpContext.User
                .FindFirstValue(ClaimTypes.Role);
            AccessRight roleAccessRights = null;

            using (_dbContext)
            {
                roleAccessRights = _dbContext.AccessRights
                    .Include(ar => ar.Page)
                    .FirstOrDefault(ar =>
                        ar.RoleId.Equals(userRoleId) &&
                        ar.Page.Name.Equals(controller));
            }

            if (roleAccessRights != null &&
                roleAccessRights.IsAvailable)
            {
                if (controller.Equals("Home") || action.Equals("Index"))
                    return;
                else
                {
                    /*roleAccessRights.ComponentsPermissions
                        .Where(r => r.Key == )*/
                }
            }

            throw new UnauthorizedAccessException("Отказано в доступе.");
        }
    }
}