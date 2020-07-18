using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models
{
    public class CashOutlay
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int Amount { get; set; } = 0;

        public int SourceUserId { get; set; }
        public int DestinationUserId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool IsRemove { get; set; } = false;
    }
}
