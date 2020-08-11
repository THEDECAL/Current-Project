using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyBilling.Helpers
{
    static public class ControllerHelper
    {
        static public Dictionary<string, string> GetControllersNames()
        {
            return Assembly.GetAssembly(typeof(CustomController)).GetTypes()
                .Where(type => type.IsSubclassOf(typeof(CustomController)) &&
                    !type.Name.Equals("HomeController")).
                Select(type =>
                {
                    var displayNamrAtt = type.GetCustomAttributes(typeof(DisplayNameAttribute))
                                .FirstOrDefault() as DisplayNameAttribute;
                    return KeyValuePair.Create(
                        type.Name.Replace("Controller", ""), displayNamrAtt.DisplayName);
                }).ToDictionary(t => t.Key, t => t.Value);
        }
    }
}
