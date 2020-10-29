using EasyBilling.Helpers;
using EasyBilling.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Models
{
    public class PaginationState
    {
        /// <summary>
        /// Максимально допустимое количество строк на страницу
        /// </summary>
        const int MAX_PAGE_ROWS_CNT = 100;

        private string _sortField = "Id";
        private int _rwsCntPerPg = 10;
        private int _currPage = 1;
        private string _srchText = string.Empty;
        private string _fltrField = string.Empty;

        public PaginationState() { }

       /// <summary>
       /// Поле/Столбец сортировки
       /// </summary>
        public string SortField
        {
            get => _sortField;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _sortField = value;
                }
            }
        }
        /// <summary>
        /// Сортировка по убыванию если false иначе по возрастанию
        /// </summary>
        public bool IsOrderByDesc { get; set; }
        /// <summary>
        /// Текущая выбранная страница
        /// </summary>
        public int CurrentPage
        {
            get => _currPage;
            set
            {
                if (value > 0)
                {
                    _currPage = value;
                }
            }
        }
        /// <summary>
        /// Количетсво строк на страницу
        /// </summary>
        public int RowsCountPerPage
        {
            get => _rwsCntPerPg;
            set
            {
                if (value > 0 && value <= MAX_PAGE_ROWS_CNT)
                {
                    _rwsCntPerPg = value;
                }
            }
        }
        /// <summary>
        /// Текст строки поиска
        /// </summary>
        public string SearchText
        {
            get => _srchText;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    _srchText = value.Trim().ToLower();
                }
            }
        }
        /// <summary>
        /// Поле/Столбец фильтр поиска
        /// </summary>
        public string FilterField
        {
            get => _fltrField;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _fltrField = value;
                }
            }
        }

        /// <summary>
        /// Количество пропускаемых элементов в зависимости от текущей страницы
        /// </summary>
        public int ToSkip { get => (_currPage - 1) * _rwsCntPerPg; }

        /// <summary>
        /// Фильтр поиска пустой?
        /// </summary>
        /// <returns></returns>
        public bool IsFilterFieldEmpty() => string.IsNullOrWhiteSpace(FilterField);
        /// <summary>
        /// Строка поиска пустая?
        /// </summary>
        /// <returns></returns>
        public bool IsSearchTextEmpty() => string.IsNullOrWhiteSpace(SearchText);

        public void ChangeState(PaginationState fromObj) => fromObj.Clone(this);
    }
}
