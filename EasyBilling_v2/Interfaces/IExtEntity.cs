using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace EasyBilling.Interfaces
{
    public interface IExtEntity : IExtEntity<int> { }

    public interface IExtEntity<TPKey>
    {
        public TPKey Id { get; set; }
        public DateTime DateOfCreation { get; set; }
        public DateTime? DateOfUpdate { get; set; }
        public bool IsRemove { get; set; }

        public bool Equals(object obj);
        public int GetHashCode();
    }

    public interface IExtEnumEntity : IExtEnumEntity<int> { }

    public interface IExtEnumEntity<TPKey> : IExtEntity<TPKey>
    {
        public string Name { get; set; }
        public string LocalizedName { get; set; }
    }
}
