using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EasyBilling.Models.Pocos
{
    public class AccessRight
    {
        [DisplayName("№")]
        public int Id { get; set; }
        [Required]
        //public string RoleId { get; set; }
        [DisplayName("Роль")]
        public IdentityRole Role { get; set; }
        [Required]
        [DisplayName("Имя контроллера")]
        public string ControllerName { get; set; }
        [Required]
        [DisplayName("Разрешение")]
        public bool IsAvailable { get; set; }
    }
}