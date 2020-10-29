using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyBilling.Helpers
{
    public static class ObjectHelper
    {
        static public string GetCurrentMethod([NotNull] this StackTrace obj) =>obj.GetFrame(1).GetMethod().Name;
        static public T Copy<T>([NotNull] this T from, params string[] excl) where T : notnull
        {
            var copy = Activator.CreateInstance<T>();
            from.Clone(copy, excl);
            return copy;
        }
        static public void Clone<T>([NotNull] this T from, [NotNull] T to, params string[] excl) where T : notnull
        {
            from.GetProps(excl).ToList().ForEach((p) => p.SetValue(to, p.GetValue(from)));
        }

        static public bool Compare<T>([NotNull] this T o1, [NotNull] T o2, params string[] excl) where T : notnull
            => o1.GetProps(excl).All((p) => p.GetValue(o1).StrChk().Equals(p.GetValue(o2).StrChk()));

        static public IEnumerable<PropertyInfo> GetProps<T>([NotNull] this T obj, params string[] excl) where T : notnull
        {
            var props = typeof(T).GetType().GetProperties();
            return ((excl == null)
                    ? props
                    : props.Where((p) => excl.Any((ep) => ep.StrChk().Equals(p.Name.StrChk()))))
                .AsEnumerable();
        }

        static public string StrChk(this object obj)
            => (string.IsNullOrWhiteSpace(obj.ToString()))
                ? string.Empty : obj.ToString().ToLower().Trim();

        static public string FirstCharToUpper([NotNull][MinLength(3)] this string str) => str[0].ToString().ToUpper() + str.Substring(1, str.Length);
    }
}
