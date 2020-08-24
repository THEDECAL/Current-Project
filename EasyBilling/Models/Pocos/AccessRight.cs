using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EasyBilling.Models.Pocos
{
    public class AccessRight
    {
        private Role _role;
        private ControllerName _controller;

        [DisplayName("#")]
        public int Id { get; set; }

        [Required (ErrorMessage = "Не выбрана роль")]
        [DisplayName("Роль*")]
        [Remote(action: "CheckRoleExist", controller: "AccessRights", ErrorMessage = "Выбранная роль не существует")]
        public Role Role { get => _role ?? new Role(); set => _role = value; }

        [Required (ErrorMessage = "Не выбрано название страницы")]
        [Remote(action: "CheckCntrlExist", controller: "AccessRights", ErrorMessage = "Выбранная страница не существует")]
        [DisplayName("Название страницы*")]
        public ControllerName Controller { get => _controller ?? new ControllerName(); set => _controller = value; }

        [Required (ErrorMessage = "Не выбрано разрешение")]
        [DisplayName("Разрешение*")]
        public bool IsAvailable { get; set; }
    }
}