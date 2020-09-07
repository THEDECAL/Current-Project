using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyBilling.Models
{
    public class ActionRight
    {
        public string Name { get; }
        public string LocalizedName { get; }
        public bool IsAvailable { get; set;  } = true;

        public ActionRight(
            [NotNull] string name = "",
            [NotNull] string localizedName = "",
            bool isAvailable = true)
        {
            LocalizedName = localizedName;
            Name = name;
        }

        public override string ToString()
            => (!string.IsNullOrWhiteSpace(LocalizedName) ? LocalizedName : Name) + "; ";
    }
}
