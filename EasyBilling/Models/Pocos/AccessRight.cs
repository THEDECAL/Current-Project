using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EasyBilling.Models.Pocos
{
    public class AccessRight
    {
        private IdentityRole _role;
        [DisplayName("№")]
        public int Id { get; set; }
        [Required (ErrorMessage = "Не выбрана роль")]
        [DisplayName("Роль")]
        [Remote(action: "CheckRoleExist", controller: "AccessRights", ErrorMessage = "Выбранная роль не существует")]
        public IdentityRole Role { get => _role ?? new IdentityRole(); set => _role = value; }
        //public IdentityRole Role { get; set; }
        [Required (ErrorMessage = "Не выбрано название страницы")]
        [Remote(action: "CheckCntrlExist", controller: "AccessRights", ErrorMessage = "Выбранная страница не существует")]
        [DisplayName("Название страницы")]
        public string ControllerName { get; set; }
        [Required (ErrorMessage = "Не выбрано разрешение")]
        [DisplayName("Разрешение")]
        public bool IsAvailable { get; set; }
    }
}