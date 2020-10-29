using EasyBilling.Attributes;
using EasyBilling.Data;
using EasyBilling.HtmlHelpers;
using EasyBilling.Models;
using EasyBilling.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyBilling.ViewModels
{
    public class TableDataViewModel<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet;
        private Type _type;
        private Func<TEntity, bool> _filter;
        public TableHtmlHelper<TEntity> TableHelper { get; private set; }
        public string UrlPath { get; }
        public PaginationState State { get; private set; }
        public string[] IncludeFields { get; private set; }
        public string[] ExcludeFields { get; private set; }
        public List<TEntity> Data { get; private set; }
        public int AmountPage { get; private set; }
        public bool IsHaveNextPage { get => ( State.CurrentPage + 1 > AmountPage) ? false : true; }
        public bool IsHavePreviousPage { get => ( State.CurrentPage - 1 < 1) ? false : true; }
        public int RowsCount { get; set; }

        public TableDataViewModel(IServiceScopeFactory scopeFactory,
            PaginationState state,
            string urlPath,
            Func<TEntity, bool> filter = null,
            string[] includeFields = null,
            string[] excludeFields = null)
        {
            var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;
            var dbContext = sp.GetRequiredService<BillingDbContext>();

            _dbSet = dbContext.Set<TEntity>();
            _type = typeof(TEntity);
            _filter = filter;
            TableHelper = new TableHtmlHelper<TEntity>(this);
            UrlPath = urlPath;
            State = state ?? new PaginationState();
            IncludeFields = includeFields ?? new string[0];
            ExcludeFields = excludeFields ?? new string[0];
            RowsCount = _dbSet.Count();
            AmountPage = (int)Math.Ceiling(_dbSet.Count() / (double)State.RowsCountPerPage);
            State.CurrentPage = (State.CurrentPage > 0 && State.CurrentPage <= AmountPage)
                ? State.CurrentPage : 1;
            Data = new List<TEntity>();

            //GetData();
        }

        private void GetData()
        {
            var searchFunc = new Func<TEntity, bool>((o) =>
                    o.GetType().GetProperties().Any(p => {
                        var value = p.GetValue(o, null);
                        return (value == null)
                            ? false
                            : value.ToString().ToLower().Contains(State.SearchText.ToLower());
                        }));
            try
            {
                IQueryable<TEntity> queryQ = _dbSet.AsQueryable().AsNoTracking();
                if (queryQ.Count() > 0)
                {
                    //Подключение связанных объектов
                    foreach (var item in IncludeFields)
                    {
                        try
                        {
                            queryQ = queryQ.Include(item);
                        }
                        catch (Exception ex)
                        { Console.WriteLine(ex.StackTrace); }
                    }

                    IEnumerable<TEntity> queryE = null;
                    //Выбераем поисковый запрос и фильтруем
                    try
                    {
                        if (_filter == null)
                            queryE = queryQ.Where(searchFunc);
                        else
                            queryE = queryQ.Where(_filter).Where(searchFunc);

                        RowsCount = queryE.Count();
                        AmountPage = (int)Math.Ceiling(queryE.Count() / (double)State.RowsCountPerPage);
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.StackTrace); }

                    //Сортировка по выбранному столбцу
                    try
                    {
                        var prop = _type.GetProperties()
                            .FirstOrDefault(p => p.Name.ToLower()
                            .Equals(State.SortField.ToLower()));

                            queryE = (State.IsOrderByDesc)
                                ? queryE.OrderByDescending(p => prop.GetValue(p, null).ToString()).ToList()
                                : queryE.OrderBy(p => prop.GetValue(p, null).ToString()).ToList();
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.StackTrace); }

                    //Выбераем данные для текущей страницы (пагинация)
                    try
                    {
                        Data.AddRange(queryE.Skip((State.CurrentPage - 1) * State.RowsCountPerPage).Take(State.RowsCountPerPage).ToList());
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.StackTrace); }
                }
            }
            catch (Exception ex)
            { Console.WriteLine(ex.StackTrace); }
        }

        public async Task<List<Dictionary<string,string>>> GetDataDicAsync()
        {
            GetData();
            return await Task.Run(() => Data.Select(o =>
                {
                    Type type = o.GetType();
                    var props = type.GetProperties();
                    var dic = new Dictionary<string, string>();

                    foreach (var item in props)
                    {
                        var noShowAttr = item.GetCustomAttribute<NoShowInTableAttribute>();
                        if (!ExcludeFields.Any(f => f.Equals(item.Name)) &&
                            noShowAttr == null)
                        {
                            var val = item.GetValue(o);
                            dic.Add(item.Name, (val != null) ? val.ToString() : "");
                        }
                    }
                    return dic;
                }).ToList());
        }
    }
}
