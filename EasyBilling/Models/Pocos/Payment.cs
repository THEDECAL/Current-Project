using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models.Pocos
{
    public class Payment
    {
        [DisplayName("#")]
        public int Id { get; set; }

        [DisplayName("Дата")]
        public DateTime Date { get; set; } = DateTime.Now;

        [DisplayName("Отправитель")]
        public Profile SourceProfile { get; set; }

        [DisplayName("Получатель")]
        public Profile DestinationProfile { get; set; }

        [DisplayName("Роль")]
        public Role Role { get; set; }

        [DisplayName("Сумма*")]
        [Required]
        public double Amount { get; set; }

        [DisplayName("Комментарий")]
        public string Comment { get; set; }
    }
}