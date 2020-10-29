using EasyBilling.Data;
using EasyBilling.Interfaces;
using EasyBilling.Models;
using EasyBilling.Helpers;
using EasyBilling.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBilling.Services
{
    public class EntityRepository<TEntity> : IRepoCrud<TEntity> where TEntity : ExtEntity
    {
        private readonly ExtController<TEntity> _prnt;

        public EntityRepository([NotNull] ExtController<TEntity> prnt)
        {
            _prnt = prnt;
        }

        #region => RepoCrud
        [HttpPost]
        public async Task<bool> Add([NotNull] TEntity model)
        {
            var method = new StackTrace().GetCurrentMethod();
            try
            {
                _prnt.ModelState.AddModelError(method, string.Empty);

                if (_prnt.ModelState.IsValid)
                {
                    await _prnt.DbSet.Add(model).Context.SaveChangesAsync();
                    _prnt.ModelState.Remove(method);

                    return true;
                }
            }
            catch (DbUpdateException ex)
            { Debug.WriteLine(ex.StackTrace); }
            return false;
        }

        [HttpPost]
        public async Task<TEntity> Get(int id)
        {
            return await _prnt.DbSet
                .SkipRemoved()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        [HttpPost]
        public async Task<IEnumerable<TEntity>> Find(PaginationState pgState = null)
        {
            try
            {
                return await Task.Run(() => _prnt.DbSet
                    .SkipRemoved()
                    .SkipTake(pgState ?? _prnt.PgnState)
                    .ExtWhere(pgState ?? _prnt.PgnState));
            }
            catch (Exception ex)
            { Debug.WriteLine(ex.StackTrace); }

            return null;
        }

        [HttpPost]
        public async Task<bool> Change([NotNull] TEntity model)
        {
            var method = new StackTrace().GetCurrentMethod();

            try
            {
                _prnt.ModelState.AddModelError(method, string.Empty);

                if (_prnt.ModelState.IsValid)
                {
                    var oldItem = await Get(model.Id);
                    if (!ObjectHelper.Compare(oldItem, model, "Id"))
                    {
                        _prnt.DbContext.Entry(oldItem).CurrentValues.SetValues(model);
                        await _prnt.DbContext.SaveChangesAsync();
                    }
                    _prnt.ModelState.Remove(method);
                    return true;
                }
            }
            catch (DbUpdateException ex)
            { Debug.WriteLine(ex.StackTrace); }
            return false;
        }

        [HttpPost]
        public async Task<bool> Remove(int id)
        {
            try
            {
                var oldItem = await Get(id);
                oldItem.IsRemove = true;
                if (oldItem != null)
                {
                    await Change(oldItem);
                    return true;
                }
            }
            catch (DbUpdateException ex)
            { Debug.WriteLine(ex.StackTrace); }
            return false;
        }
        #endregion
    }

    public static class CrudLINQ
    {
        public static IEnumerable<TEntity> ExtWhere<TEntity>(this IEnumerable<TEntity> items, PaginationState state) where TEntity : ExtEntity
        {
            var func = new Func<TEntity, bool>((obj) =>
            {
                var type = obj.GetType();
                if (!state.IsSearchTextEmpty())
                {
                    var st = state.SearchText.ToLower().Trim();
                    if (state.IsFilterFieldEmpty())
                    {
                        var sb = new StringBuilder();
                        type.GetProperties().ToList().ForEach(p =>
                        {
                            var val = p.GetValue(obj, null)?.ToString() ?? string.Empty;
                            sb.Append(val.ToLower().Trim()).Append(' ');
                        });
                        return sb.ToString().Contains(st);
                    }
                    else
                    {
                        var ffVal = type.GetProperty(state.FilterField).GetValue(obj, null)?.ToString() ?? string.Empty;
                        return ffVal.ToLower().Trim().Contains(st);
                    }
                }
                return true;
            });

            return items.Where(func).ExtOrder(state);
        }

        public static IQueryable<TEntity> SkipTake<TEntity>(this IQueryable<TEntity> items, PaginationState state) where TEntity : ExtEntity
            => items.Skip(state.ToSkip).Take(state.RowsCountPerPage);

        public static IQueryable<TEntity> SkipRemoved<TEntity>(this IQueryable<TEntity> items) where TEntity : ExtEntity
            => items.Where(o => !(o as IExtEnumEntity).IsRemove);

        public static IEnumerable<TEntity> ExtOrder<TEntity>(this IEnumerable<TEntity> items, PaginationState state) where TEntity : ExtEntity
        {
            var func = new Func<TEntity, object>(obj => obj.GetType().GetProperty(state.SortField)?.GetValue(obj, null)?.ToString() ?? string.Empty);

            return (state.IsOrderByDesc)
            ? items.OrderByDescending(func).AsEnumerable()
            : items.OrderBy(func).AsEnumerable();
        }

    }
}
