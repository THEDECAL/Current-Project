using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models
{
    public abstract class Permissions
    {
        public bool IsCreate { get; set; } = true;
        public bool IsRead { get; set; } = true;
        public bool IsUpdate { get; set; } = true;
        public bool IsDelete { get; set; } = true;
    }
}