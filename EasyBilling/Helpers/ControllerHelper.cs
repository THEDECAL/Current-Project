using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyBilling.Helpers
{
    static public class ControllerHelper
    {
        /// <summary>
        /// Получение словаря всех контроллеров где:
        /// key = Имя класса контроллера
        /// value = Локализированное имя контроллера
        /// </summary>
        /// <returns></returns>
        static public async Task<Dictionary<string, string>> GetControllersNamesAsync()
        {
            return await Task.Run(() =>
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
            });
        }

        /// <summary>
        /// Проверка на существование контроллера
        /// </summary>
        /// <param name="cntrlName"></param>
        /// <returns></returns>
        //static public async Task<bool> IsExistAsync([NotNull] string cntrlName)
        //{
        //    var names = await GetControllersNamesAsync();
        //    return names.Keys.Any(k => k.Equals(cntrlName));
        //}

        //static public async Task<string> GetLocalizedName([NotNull] string cntrlName)
        //{
        //    return await Task.Run(() =>
        //    {
        //        var type = Type.GetType("EasyBilling.Controllers." + cntrlName + "Controller");
        //        var displayNamrAtt = type.GetCustomAttributes(typeof(DisplayNameAttribute))
        //                    .FirstOrDefault() as DisplayNameAttribute;
        //        return displayNamrAtt.DisplayName;
        //    });
        //}
    }
}
