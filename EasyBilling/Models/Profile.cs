using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models
{
    public class Profile
    {
        public enum State { Disable, Enable, Frozen };
        public int Id { get; set; }
        public int CurrentState { get; set; } = (int)State.Enable;
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public double AmountOfCash { get; set; } = 0;
        public int TariffId { get; set; }
        public Tariff Tarrif { get; set; }
        public DateTime DateBeginOfUseOfTarrif { get; set; }
        public int UsedTraffic { get; set; } = 0;
        public string CustomField1 { get; set; } = string.Empty;
        public string CustomField2 { get; set; } = string.Empty;
        public string CustomField3 { get; set; } = string.Empty;
        public string CustomField4 { get; set; } = string.Empty;
        public string CustomField5 { get; set; } = string.Empty;
        public DateTime DateOfCreation { get; set; } = DateTime.Now;
        public DateTime DateOfUpdate { get; set; } = DateTime.Now;
        public bool IsHolded { get; set; } = false;
    }
}