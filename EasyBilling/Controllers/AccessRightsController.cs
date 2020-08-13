using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Helpers;
using EasyBilling.Models.Pocos;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EasyBilling.Controllers
{
    [Authorize]
    [CheckAccessRights]
    [DisplayName("Права доступа")]
    public class AccessRightsController : CustomController
    {
        private readonly BillingDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IServiceScopeFactory _scopeFactory;
        public AccessRightsController(BillingDbContext dbContext,

            RoleManager<IdentityRole> roleManager,
            IServiceScopeFactory scopeFactory)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _scopeFactory = scopeFactory;
        }

        [HttpGet]
        public IActionResult Index(string sort = "Id",
            SortType sortType = SortType.ASC,
            int page = 1,
            int pageSize = 10)
        {
            ViewData["Title"] = DisplayName;

            var dvm = new DataViewModel<AccessRight>(_scopeFactory,
                includeField1: "Role",
                sortType: sortType,
                sortField: sort,
                page: page,
                pageSize: pageSize
            );
            var roles = _roleManager.Roles.ToList();
            var cntrlsNames = ControllerHelper.GetControllersNames();
            var model = new AccessRightsViewModel(dvm, roles, cntrlsNames);

            return View(model: model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccessRight(AccessRight rights)
        {
            if (rights != null)
            {
                await _dbContext.AccessRights.AddAsync(rights);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAccessRight(AccessRight rights)
        {
            if (rights != null)
            {
                await Task.Run(() => _dbContext.Update(rights));
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccessRight(int? id)
        {
            if (id != null)
            {
                await Task.Run(() => _dbContext.Remove(id.Value));
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(IdentityRole role)
        {
            if (role != null)
            {
                await _roleManager.CreateAsync(role);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(IdentityRole role)
        {
            if (role != null)
            {
                await Task.Run(() => _dbContext.Roles.Update(role));
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                await _roleManager.DeleteAsync(new IdentityRole(id));
            }

            return RedirectToAction("Index");
        }
    }
}
