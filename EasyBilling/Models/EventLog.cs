using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models
{
    public class EventLog
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsRemove { get; set; } = false;
    }
}