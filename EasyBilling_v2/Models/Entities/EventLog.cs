using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models.Entities
{
    public class EventLog : ExtEntity
    {
        //public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        //public DateTime Date { get; set; } = DateTime.Now;
    }
}