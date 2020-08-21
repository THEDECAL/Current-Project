using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models
{
    public interface ICustomControllerCrud<T> where T : class
    {
        public Task<IActionResult> AddUpdateForm(int? id = null);
        public Task<IActionResult> Create(T obj);
        public Task<IActionResult> Update(T obj);
        public Task<IActionResult> Delete(int? id = null);
        public Task ServerSideValidation(T obj);
    }
}
