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
    public class DataViewModel<T> where T : class
    {
        const int MAX_PAGE_SIZE = 100;
        private readonly DbSet<T> _dbSet;
        private int _currentPage = 1;
        private int _pageSIze = 15;
        private string _include1;
        private string _include2;
        public string PreviousSort { get; set; } = string.Empty;
        public string CurrentSort { get; set; } = "Id";
        public List<T> Data { get; private set; }
        public int AmountPage
        {
            get => (int)Math.Ceiling(_dbSet.Count() / (double)PageSize);
        }
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage > 0 &&
                    _currentPage <= AmountPage)
                    _currentPage = value;
            }
        }
        public int PageSize
        {
            get => _pageSIze;
            set
            {
                if (value <= MAX_PAGE_SIZE)
                    _pageSIze = value;
            }
        }
        public DataViewModel(IServiceScopeFactory scopeFactory, string include1 = null, string include2 = null)
        {
            var scope = scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            var dbContext = sp.GetRequiredService<BillingDbContext>();
            _dbSet = dbContext.Set<T>();
            _include1 = include1;
            _include2 = include2;
        }

        public void GetData()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_include1))
                    _dbSet.Include(_include1);
                else if (!string.IsNullOrWhiteSpace(_include2))
                    _dbSet.Include(_include2);
                else if (!string.IsNullOrWhiteSpace(_include1) &&
                    !string.IsNullOrWhiteSpace(_include2))
                    _dbSet.Include(_include1).Include(_include2);

                //Выбераем данные для текущей страницы
                var data = _dbSet.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
                //Data = data.Sort().
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
