using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int SourceUserId { get; set; }
        public int DestinationUserId { get; set; }
        public double Amount { get; set; } = 0;
        public string Comment { get; set; } = string.Empty;
        public bool IsRemove { get; set; } = false;
    }
}