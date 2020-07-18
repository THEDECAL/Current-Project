using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models
{
    public class Device
    {
        public enum State { Disabled, InWorked, OnStock, Decommissioned, OnRepair };
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int CurrentState { get; set; } = (int)State.Disabled;
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string CustomField1 { get; set; } = string.Empty;
        public string CustomField2 { get; set; } = string.Empty;
        public string CustomField3 { get; set; } = string.Empty;
        public string CustomField4 { get; set; } = string.Empty;
        public string CustomField5 { get; set; } = string.Empty;
        public bool IsRemove { get; set; } = false;
    }
}