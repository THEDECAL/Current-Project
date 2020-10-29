
using EasyBilling.Attributes;
using EasyBilling.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models
{
    public class ExtEntity: IExtEntity
    {
        [DisplayName("#")]
        public int Id { get; set; }

        [DisplayName("Дата создания")]
        [NoShowInTable]
        public DateTime DateOfCreation { get; set; } = DateTime.Now;

        [DisplayName("Дата обновления")]
        [NoShowInTable]
        public DateTime? DateOfUpdate { get; set; }

        [DisplayName("Удалён?")]
        [NoShowInTable]
        public bool IsRemove { get; set; } = false;

        public override bool Equals(object obj) => (obj is ExtEntity && Id != 0) ? (obj as ExtEntity).Id == Id : false;

        public override int GetHashCode() => Id;
    }

    public abstract class ExtEnumEntity : ExtEntity, IExtEnumEntity
    {
        [Required]
        [RegularExpression(@"[A-Za-z0-9]", ErrorMessage = "Недопустимые символы (только A-Z, a-z, 0-9)")]
        [DisplayName("Название латиницей")]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Локализированное имя*")]
        public string LocalizedName { get; set; } = string.Empty;

        public override string ToString() => (string.IsNullOrWhiteSpace(LocalizedName)) ? Name : LocalizedName;
    }
}
