using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EasyBilling.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyBilling.Models.Entities
{
    public class Profile : ExtEntity
    {
        private User _user;
        private Tariff _tariff;

        [DisplayName("Логин")]
        [Required]
        public User User { get => _user ?? (User = new User()); set => _user = value; }
        [NoShowInTable]
        public string UserId { get; set; }

        [DisplayName("Имя*")]
        [Required]
        public string FirstName { get; set; }

        [DisplayName("Фамилия*")]
        [Required]
        public string SecondName { get; set; }

        [DisplayName("Отчество")]
        public string Patronymic { get; set; }

        [DisplayName("Адрес подключения*")]
        [Required]
        public string Address { get; set; }

        [DisplayName("Счёт")]
        [Required]
        public double BalanceOfCash { get; set; } = 0;

        [DisplayName("Тариф*")]
        [Required]
        //[Remote(action: "CheckTariff", controller: "Users", ErrorMessage = "Выбранный тариф не существует")]
        public Tariff Tariff { get => _tariff ?? (Tariff = new Tariff()); set => _tariff = value; }
        [NoShowInTable]
        public int TariffId { get; set; }

        [DisplayName("Дата начала использования тарифа")]
        public DateTime? StartUsedOfTariff { get; set; }

        [DisplayName("Использованный трафик")]
        public int TrafficUsed { get; set; } = 0;

        public string CustomProfileField1 { get; set; }
        public string CustomProfileField2 { get; set; }
        public string CustomProfileField3 { get; set; }

        [DisplayName("Дата последнего входа")]
        public DateTime LastOfLogin { get; set; }

        [DisplayName("Заморожен?")]
        [Required]
        public bool IsHolded { get; set; } = false;

        [DisplayName("Включен?")]
        [Required]
        public bool IsEnabled { get; set; } = true;

        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        public override string ToString() => $"{SecondName} {FirstName} {User}";
    }
}