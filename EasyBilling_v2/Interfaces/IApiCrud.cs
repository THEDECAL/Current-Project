using EasyBilling.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBillingV2.Interfaces
{
    public interface IApiCrud<TEntity>
    {
        public Task<ActionResult> Add(TEntity model);

        public Task<ActionResult> Change(TEntity model);

        public Task<ActionResult> Remove(int id);

        public Task<ActionResult> Get(int id);

        public Task<ActionResult> Find(PaginationState pgState = null);
    }
}
