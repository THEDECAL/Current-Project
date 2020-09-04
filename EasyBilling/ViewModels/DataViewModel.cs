using EasyBilling.Data;
using EasyBilling.HtmlHelpers;
using EasyBilling.Models;
using EasyBilling.Models.Pocos;
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
    [Flags]
    public enum SortType { ASC, DESC }
    public class DataViewModel<T> where T : class
    {
        private readonly DbSet<T> _dbSet;
        private Type _type;
        //private int _page;
        //private int _pageSize;
        private Func<T, bool> _filter;
        public TableHtmlHelper<T> TableHelper { get; private set; }
        public string ControllerName { get; private set; }
        public ControlPanelSettings Settings { get; private set; }
        public string[] IncludeFields { get; private set; }
        public string[] ExcludeFields { get; private set; }
        //public string SortField { get; private set; }
        //public SortType SortType { get; private set; }
        public List<T> Data { get; private set; }
        public int AmountPage { get; private set; }
        //public string SearchRequest { get; private set; }
        //public int Page
        //{
        //    get => _page;
        //    private set
        //    {
        //        if (value > 0 && value <= AmountPage)
        //            _page = value;
        //    }
        //}
        //public int PageSize
        //{
        //    get => _pageSize;
        //    private set
        //    {
        //        if (value <= MAX_PAGE_SIZE)
        //            _pageSize = value;
        //    }
        //}
        public bool IsHaveNextPage { get => ( Settings.CurrentPage + 1 > AmountPage) ? false : true; }
        public bool IsHavePreviousPage { get => ( Settings.CurrentPage - 1 < 1) ? false : true; }

        public DataViewModel(IServiceScopeFactory scopeFactory,
            string controllerName,
            ControlPanelSettings settings,
            Func<T, bool> filter = null,
            string[] includeFields = null,
            string[] excludeFields = null)
            //string sortField = "Id",
            //SortType sortType = SortType.ASC,
            //int page = 1,
            //int pageSize = 10,
            //string searchRequest = "")
        {
            var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;
            var dbContext = sp.GetRequiredService<BillingDbContext>();

            _dbSet = dbContext.Set<T>();
            _type = typeof(T);
            _filter = filter;
            TableHelper = new TableHtmlHelper<T>(this);
            ControllerName = controllerName;
            Settings = settings ?? new ControlPanelSettings();
            IncludeFields = includeFields ?? new string[0];
            ExcludeFields = excludeFields ?? new string[0];
            //SearchRequest = (searchRequest == null )?"":searchRequest.ToLower();
            //IncludeFields = includeFields ?? new string[0];
            //ExcludeFields = excludeFields ?? new string[0];
            //SortField = sortField;
            //SortType = sortType;
            //PageSize = pageSize;
            AmountPage = (int)Math.Ceiling(_dbSet.Count() / (double)Settings.PageRowsCount);
            Settings.CurrentPage = (Settings.CurrentPage > 0 && Settings.CurrentPage <= AmountPage)
                ? Settings.CurrentPage : 1;
            Data = new List<T>();
            //Page = page;

            GetData();
        }

        private void GetData()
        {
            var searchFunc = new Func<T, bool>((o) =>
                    o.GetType().GetProperties().Any(p => {
                        var value = p.GetValue(o, null);
                        return (value == null)
                            ? false
                            : value.ToString().ToLower().Contains(Settings.SearchText.ToLower());
                        }));
            try
            {
                IQueryable<T> queryQ = _dbSet.AsQueryable();
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

                    IEnumerable<T> queryE = null;
                    //Выбераем поисковый запрос и фильтруем
                    try
                    {
                        if (_filter == null)
                            queryE = queryQ.Where(searchFunc);
                        else
                            queryE = queryQ.Where(_filter).Where(searchFunc);

                        AmountPage = (int)Math.Ceiling(queryE.Count() / (double)Settings.PageRowsCount);
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.StackTrace); }

                    //Сортировка по выбранному столбцу
                    try
                    {
                        var prop = _type.GetProperties()
                            .FirstOrDefault(p => p.Name.ToLower()
                            .Equals(Settings.SortField.ToLower()));

                        if (Settings.SortType == SortType.DESC)
                            queryE = queryE.OrderByDescending(p => prop.GetValue(p, null).ToString()).ToList();
                        else
                            queryE = queryE.OrderBy(p => prop.GetValue(p, null).ToString()).ToList();
                    }
                    catch (Exception ex)
                    { Console.WriteLine(ex.StackTrace); }

                    //Выбераем данные для текущей страницы (пагинация)
                    try
                    {
                        Data.AddRange(queryE.Skip((Settings.CurrentPage - 1) * Settings.PageRowsCount).Take(Settings.PageRowsCount).ToList());
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
            return await Task.Run(() => Data.Select(o =>
                {
                    Type type = o.GetType();
                    var props = type.GetProperties();
                    var dic = new Dictionary<string, string>();

                    foreach (var item in props)
                    {
                        if (!ExcludeFields.Any(f => f.Equals(item.Name)))
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
