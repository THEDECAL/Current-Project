using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePoker.Models
{
    public class Winner
    {
        public int Id { get; set; }
        public DateTime WinDate { get; set; }
        public string CombinationName { get; set; }
        public int Bank { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
