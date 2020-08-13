using EasyBilling.Helpers;
using EasyBilling.HtmlHelpers;
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
        public TableHtmlHelper<AccessRight> TableHelper { get; private set; }
        public DataViewModel<AccessRight> DataViewModel { get; private set; }
        public Dictionary<string,string> ControllersNames { get; private set; }
        public List<IdentityRole> Roles { get; private set; }

        public AccessRightsViewModel(DataViewModel<AccessRight> data,
            List<IdentityRole> roles,
            Dictionary<string, string> controllersNames)
        {
            DataViewModel = data;
            Roles = roles;
            ControllersNames = controllersNames;
            TableHelper = new TableHtmlHelper<AccessRight>(data);
        }
    }
}
