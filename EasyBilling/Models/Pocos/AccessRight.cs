using EasyBilling.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasyBilling.Models.Pocos
{
    public class AccessRight
    {
        readonly private Dictionary<Component, Permissions> _componentsPermissions
            = new Dictionary<Component, Permissions>();

        public int Id { get; set; }
        public string RoleId { get; set; }
        public IdentityRole Role { get; set; }
        public int PageId { get; set; }
        public Page Page { get; set; }
        public bool IsAvailable { get; set; } = false;
        public string ComponentsPermissionsJson { get; private set; }
        [NotMapped]
        public Dictionary<Component, Permissions> ComponentsPermissions
        {
            get
            {
                if (_componentsPermissions.Count == 0 &&
                    !String.IsNullOrEmpty(ComponentsPermissionsJson))
                {
                    try
                    {
                        var dic = JsonConvert
                            .DeserializeObject<Dictionary<Component, Permissions>>
                            (ComponentsPermissionsJson);
                        if (dic != null && dic.Count > 0)
                        {
                            foreach (var item in dic)
                            {
                                ComponentsPermissions.Add(item.Key, item.Value);
                            }
                        }
                    }
                    catch (JsonSerializationException ex)
                    {
                    }
                }
                return _componentsPermissions;
            }
        }
        public void AddPermissions(Component component, Permissions permissions)
        {
            _componentsPermissions.Add(component, permissions);
            UpdateComponentsPermissionsJson();
        }

        public void DelPermissions(Component component)
        {
            _componentsPermissions.Remove(component);
            UpdateComponentsPermissionsJson();
        }

        private void UpdateComponentsPermissionsJson()
        {
            try
            {
                ComponentsPermissionsJson =
                    JsonConvert.SerializeObject(ComponentsPermissions);
            }
            catch (JsonSerializationException ex)
            {
            }
        }
    }
}