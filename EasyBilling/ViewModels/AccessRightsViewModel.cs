using EasyBilling.Models.Pocos;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.ViewModels
{
    public class AccessRightsViewModel
    {
        public Dictionary<string,string> ControllersNames { get; private set; }
        public List<AccessRight> AccessRights { get; private set; }
        public List<IdentityRole> Roles { get; private set; }

        public AccessRightsViewModel(List<AccessRight> accessRights,
            List<IdentityRole> roles, Dictionary<string, string> controllersNames)
        {
            ControllersNames = controllersNames;
            AccessRights = accessRights;
            Roles = roles;
        }
    }
}
