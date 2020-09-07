using EasyBilling.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EasyBilling.Models.Pocos
{
    public class AccessRight
    {
        private ObservableCollection<ActionRight> _rights = new ObservableCollection<ActionRight>();
        private Role _role = new Role();
        private ControllerName _controller = new ControllerName();
        private string _actionsRightsJson = "";

        [DisplayName("#")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Не выбрана роль")]
        [DisplayName("Роль*")]
        [Remote(action: "CheckRoleExist", controller: "AccessRights", ErrorMessage = "Выбранная роль не существует")]
        public Role Role { get => _role; set => _role = value ?? _role; }

        [Required(ErrorMessage = "Не выбрано название страницы")]
        [Remote(action: "CheckCntrlExist", controller: "AccessRights", ErrorMessage = "Выбранная страница не существует")]
        [DisplayName("Название страницы*")]
        public ControllerName Controller { get => _controller; set => _controller = value ?? _controller; }

        [Required(ErrorMessage = "Не выбрано разрешение")]
        [DisplayName("Разрешение*")]
        public bool IsAvailable { get; set; }

        [Required]
        [NoShowInTable]
        public string ActionsRightsJson
        {
            get => _actionsRightsJson;
            set { _actionsRightsJson = value ?? ""; DeserializeRights(); }
        }


        [DisplayName("Действия")]
        [NoShowInTable]
        [NotMapped]
        public ObservableCollection<ActionRight> Rights
        {
            get
            {
                if (_rights.Count() == 0)
                {
                    DeserializeRights();
                }
                return _rights;
            }
            set => _rights = value ?? _rights;
        }

        [DisplayName("Разрешенные действия")]
        [NotMapped]
        public string RigthsString
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var item in Rights)
                {
                    if (item.IsAvailable)
                    {
                       sb.Append(item.ToString());
                    }
                }
                return sb.ToString();
            }
        }

        public AccessRight()
        {
            DeserializeRights();

            Rights.CollectionChanged +=
                new NotifyCollectionChangedEventHandler((o, e) =>
                {
                    SerializeRights();
                });
        }

        public AccessRight(
            Role role = null,
            ControllerName controller = null,
            ObservableCollection<ActionRight> rights = null,
            bool isAvailable = false):this()
        {
            Role = role;
            Controller = controller;
            IsAvailable = isAvailable;

            AddRangeRights(rights);
        }

        private void SerializeRights()
        {
            try
            {
                _actionsRightsJson = JsonConvert.SerializeObject(Rights);
            }
            catch (JsonException ex)
            { Console.WriteLine(ex.StackTrace); }
        }

        private void DeserializeRights()
        {
            try
            {
                var rights = JsonConvert.DeserializeObject
                    <ObservableCollection<ActionRight>>(_actionsRightsJson);

                if (rights != null)
                {
                    AddRangeRights(rights);
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.StackTrace); }
        }

        public void AddRangeRights(IList<ActionRight> rights)
        {
            if (rights != null)
            {
                foreach (var item in rights)
                {
                    _rights.Add(item);
                }
                SerializeRights();
                return;
            }
            throw new ArgumentNullException();
        }

        //public ActionRight GetRight(string actionName)
        //{
        //    if (!string.IsNullOrWhiteSpace(actionName))
        //    {
        //        return Rights.FirstOrDefault(r => r.Name.Equals(actionName));
        //    }
        //    throw new ArgumentNullException();
        //}

        //public void SetRight(string actionName)
        //{
        //    if (right != null)
        //    {
        //        var oldRight = GetRight(actionName);
        //        int index = Rights.IndexOf(oldRight);
        //        if (index != -1)
        //        {
        //            Rights.Insert(index, actionName);
        //            return;
        //        }
        //    }
        //    throw new ArgumentNullException();
        //}
    }
}