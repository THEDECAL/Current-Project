using EasyBilling.Data;
using EasyBilling.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Interfaces
{
    public interface IRepoCrud<TEntity> where TEntity : IExtEntity
    {
        public Task<bool> Add([NotNull] TEntity model);

        public Task<bool> Change([NotNull] TEntity model);

        public Task<bool> Remove(int id);

        public Task<TEntity> Get(int id);

        public Task<IEnumerable<TEntity>> Find(PaginationState pgState = null);
    }
}
