using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EasyBilling.Models;
using System.Security.Principal;
using EasyBilling.Interfaces;
using EasyBilling.Attributes;

namespace EasyBilling.Models.Entities
{
    public class Role : IdentityRole//, IExtEnumEntity<string>
    {
        [DisplayName("#")]
        public new string Id { get; set; }

        [DisplayName("Страница по умолчанию*")]
        [Required(ErrorMessage = "Страницу обязательно нужно указать")]
        public ControllerName DefaultControllerName { get; set; }
        [NoShowInTable]
        public int DefaultControllerNameId { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9]", ErrorMessage = "Недопустимые символы (только A-Z, a-z, 0-9)")]
        [DisplayName("Название латиницей")]
        public new string Name { get; set; } = string.Empty;

        [DisplayName("Локализированное имя*")]
        public string LocalizedName { get; set; }

        [DisplayName("Удалён?")]
        [NoShowInTable]
        public bool IsRemove { get; set; } = false;

        public override bool Equals(object obj) => (!string.IsNullOrWhiteSpace((obj as Role).Id)) ? (obj as Role).Id.Equals(Id) : false;
        public override int GetHashCode() => base.GetHashCode();
        public override string ToString() => (string.IsNullOrWhiteSpace(LocalizedName)) ? Name : LocalizedName;
    }
}
