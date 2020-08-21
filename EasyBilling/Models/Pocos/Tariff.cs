using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models.Pocos
{
    public class Tariff
    {
        public int Id { get; set; }
        [DisplayName("Название")]
        [Required(ErrorMessage = "Не ввведено название")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Длина строки от 5 до 30 символов")]
        public string Name { get; set; } = string.Empty;
        [DisplayName("Цена")]
        [Required(ErrorMessage = "Не введена цена")]
        public double Price { get; set; } = 0;
        [DisplayName("Объём трафика")]
        public int AmountOfTraffic { get; set; } = 0;
        [DisplayName("Входящая")]
        [Required(ErrorMessage = "Не указана входящая пропускная способность")]
        [Range(100, 100000, ErrorMessage = "Допустимая пропускная способность от 100 до 100000")]
        public int BandwidthInput { get; set; } = 100000;
        [DisplayName("Исходящая")]
        [Required(ErrorMessage = "Не указана исходящая пропускная способность")]
        [Range(100, 100000, ErrorMessage = "Допустимая пропускная способность от 100 до 100000")]
        public int BandwidthOutput { get; set; } = 100000;
        [DisplayName("Количество дней")]
        [Required(ErrorMessage = "Не указано количество дней")]
        [Range(1,31, ErrorMessage = "Допустимое количество дней от 1 до 31")]
        public int AmounfOfDays { get; set; } = 28;
        public DateTime DateOfCreation { get; private set; } = DateTime.Now;
        public DateTime DateOfUpdate { get; set; }
        [DisplayName("Активность")]
        [Required(ErrorMessage = "")]
        public bool IsEnabled { get; set; } = false;
    }
}