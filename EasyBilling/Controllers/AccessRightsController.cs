using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyBilling.Controllers
{
    [Authorize]
    [CheckAccessRights]
    [DisplayName("Права доступа")]
    public class AccessRightsController : CustomController
    {
        private readonly BillingDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccessRightsController(BillingDbContext dbContext,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
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
