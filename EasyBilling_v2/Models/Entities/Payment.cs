using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models.Entities
{
    public class Payment : ExtEntity
    {
        [DisplayName("Отправитель")]
        public Profile SourceProfile { get; set; }
        public int? SourceProfileId { get; set; }

        [DisplayName("Получатель")]
        public Profile DestinationProfile { get; set; }
        public int? DestinationProfileId { get; set; }

        [DisplayName("Роль отправителя")]
        public Role Role { get; set; }
        public string RoleId { get; set; }

        [DisplayName("Сумма*")]
        [Required]
        public double Amount { get; set; }

        [DisplayName("Комментарий")]
        public string Comment { get; set; }
    }
}