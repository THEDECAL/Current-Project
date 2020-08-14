using EasyBilling.Data;
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

namespace EasyBilling.Helpers
{
    [Flags]
    public enum SortType { ASC, DESC }
    public class DataViewModel<T> where T : class
    {
        const int MAX_PAGE_SIZE = 100;
        private readonly DbSet<T> _dbSet;
        private Type _type;
        private int _page;
        private int _pageSize;
        public string IncludeField1 { get; private set; }
        public string IncludeField2 { get; private set; }
        public string SortField { get; private set; }
        public SortType SortType { get; private set; }
        public List<T> Data { get; private set; }
        public int AmountPage { get; private set; }
        public string SearchRequest { get; private set; }
        public int Page
        {
            get => _page;
            private set
            {
                if (value > 0 && value <= AmountPage)
                    _page = value;
            }
        }
        public int PageSize
        {
            get => _pageSize;
            private set
            {
                if (value <= MAX_PAGE_SIZE)
                    _pageSize = value;
            }
        }
        public bool IsHaveNextPage { get => (Page + 1 > AmountPage) ? false : true; }
        public bool IsHavePreviousPage { get => (Page - 1 < 1) ? false : true; }
        public DataViewModel(IServiceScopeFactory scopeFactory,
            string includeField1 = "",
            string includeField2 = "",
            string sortField = "Id",
            SortType sortType = SortType.ASC,
            int page = 1,
            int pageSize = 10,
            string searchRequest = "")
        {
            var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            var dbContext = sp.GetRequiredService<BillingDbContext>();
            _dbSet = dbContext.Set<T>();

            _type = typeof(T);
            SearchRequest = (searchRequest == null )?"":searchRequest.ToLower();
            IncludeField1 = includeField1;
            IncludeField2 = includeField2;
            SortField = sortField;
            SortType = sortType;
            PageSize = pageSize;
            AmountPage = (int)Math.Ceiling(_dbSet.Count() / (double)PageSize);
            Page = page;

            GetData();
        }

        private void GetData()
        {
            try
            {
                //Выбераем данные для текущей страницы (пагинация)
                var query = _dbSet.Skip((Page - 1) * PageSize).Take(PageSize);

                try
                {
                    //Подключение связанных объектов
                    if (!string.IsNullOrWhiteSpace(IncludeField1))
                        query = query.Include(IncludeField1);
                    else if (!string.IsNullOrWhiteSpace(IncludeField2))
                        query = query.Include(IncludeField2);
                    else if (!string.IsNullOrWhiteSpace(IncludeField1) &&
                        !string.IsNullOrWhiteSpace(IncludeField2))
                        query = query.Include(IncludeField1).Include(IncludeField2);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }

                //Сортировка по выбранному столбцу
                var prop = _type.GetProperties()
                    .FirstOrDefault(p => p.Name.ToLower()
                    .Equals(SortField.ToLower()));
                var searchFunc = new Func<T, bool>((o) =>
                    o.GetType().GetProperties().Any(p => p.GetValue(o, null)
                            .ToString().ToLower().Contains(SearchRequest.ToLower())));

                if (SortType == SortType.DESC)
                    Data = query.Where(searchFunc)
                        .OrderByDescending(p => prop.GetValue(p, null).ToString()).ToList();
                else
                    Data = query.Where(searchFunc)
                        .OrderBy(p => prop.GetValue(p, null).ToString()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
