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
            get => SerializeRights();
            set { _actionsRightsJson = value ?? ""; DeserializeRightsAsync().Wait(); }
        }

        [NoShowInTable]
        [NotMapped]
        public ObservableCollection<ActionRight> Rights { get => _rights; set => _rights = value ?? _rights; }

        [DisplayName("Разрешения действий")]
        [NotMapped]
        public string RigthsString { get; private set; }

        public AccessRight() { }

        public AccessRight(
            Role role = null,
            ControllerName controller = null,
            ObservableCollection<ActionRight> rights = null,
            bool isAvailable = false)
        {
            Role = role;
            Controller = controller;
            IsAvailable = isAvailable;

            AddRangeRightsAsync(rights).Wait();

            Rights.CollectionChanged +=
                new NotifyCollectionChangedEventHandler(async (o, e)
                    => await SerializeRightsAsync());
        }

        private async Task<string> SerializeRightsAsync()
            => await Task.Run(() => SerializeRights());

        private string SerializeRights()
        {
            try
            {
                return JsonConvert.SerializeObject(Rights);
            }
            catch (JsonException ex)
            { Console.WriteLine(ex.StackTrace); }

            return "";
        }

        private async Task UpdateRightsString()
            => await Task.Run(() =>
            {
                var sb = new StringBuilder();
                foreach (var item in Rights)
                {
                    sb.Append(item.ToString());
                }
                RigthsString = sb.ToString();
            });

        private async Task DeserializeRightsAsync()
            => await Task.Run(async () =>
            {
                try
                {
                    var rights = JsonConvert.DeserializeObject
                        <ObservableCollection<ActionRight>>(_actionsRightsJson);

                    if (rights != null)
                    {
                        await AddRangeRightsAsync(rights);
                        await UpdateRightsString();
                    }
                }
                catch (Exception ex)
                { Console.WriteLine(ex.StackTrace); }
            });

        public async Task AddRangeRightsAsync(IList<ActionRight> rights)
            => await Task.Run(() =>
            {
                if (rights != null)
                {
                    foreach (var item in rights)
                    {
                        Rights.Append(item);
                    }
                    return;
                }
                throw new ArgumentNullException();
            });

        public async Task<ActionRight> GetRightAsync(string actionName)
            => await Task.Run(() =>
            {
                if (!string.IsNullOrWhiteSpace(actionName))
                {
                    return Rights.FirstOrDefault(r => r.Name.Equals(actionName));
                }
                throw new ArgumentNullException();
            });

        public async Task UpdateRightAsync(ActionRight right)
        => await Task.Run(async () =>
        {
            if (right != null)
            {
                var oldRight = await GetRightAsync(right.Name);
                int? index = Rights.IndexOf(oldRight);
                if (index != null)
                {
                    Rights.Insert(index.Value, right);
                    return;
                }
            }
            throw new ArgumentNullException();
        });
    }
}