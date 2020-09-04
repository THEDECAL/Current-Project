using EasyBilling.Helpers;
using EasyBilling.ViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EasyBilling.HtmlHelpers
{
    public class TableHtmlHelper<T> where T : class
    {
        const int PAGE_COUNT = 3; //Кол-во страниц на секцию
        private Type _type;
        private DataViewModel<T> _data;
        public TableHtmlHelper(DataViewModel<T> data)
        {
            _type = typeof(T);
            _data = data;
        }
        /// <summary>
        /// Получить Html код заголовоки столбцов таблицы
        /// </summary>
        /// <returns></returns>
        public async Task<HtmlString> GetTableHeadAsync(bool isShowActions = true)
        {
            return await Task.Run(async () =>
            {
                TagBuilder thead = new TagBuilder("thead");
                TagBuilder tr = new TagBuilder("tr");
                tr.AddCssClass("table-dark");
                thead.InnerHtml.AppendHtml(tr);

                var props = _type.GetProperties();
                foreach (var item in props)
                {
                    if (!_data.ExcludeFields.Any(f => f.Equals(item.Name)))
                    {
                        var dnAtt = item.GetCustomAttribute(
                            typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                        TagBuilder th = new TagBuilder("th");

                        TagBuilder a = new TagBuilder("a");
                        th.Attributes.Add("scope", "col");
                        a.Attributes.Add("href",
                            GetHref(sort: item.Name,
                                    sortType: _data.Settings.SortType,
                                    page: _data.Settings.CurrentPage,
                                    pageSize: _data.Settings.PageRowsCount));
                        a.AddCssClass("text-white");
                        a.InnerHtml.Append((dnAtt != null) ? dnAtt.DisplayName : item.Name);
                        th.InnerHtml.AppendHtml(a);
                        tr.InnerHtml.AppendHtml(th);
                    }
                }

                if (isShowActions)
                {
                    TagBuilder thActions = new TagBuilder("th");
                    thActions.AddCssClass("text-white");
                    thActions.Attributes.Add("scope", "col");
                    thActions.InnerHtml.Append("Действия");
                    tr.InnerHtml.AppendHtml(thActions);
                }

                return await GetHtmlStringAsync(thead);
            });
        }
        /// <summary>
        /// Получить Html код панели постраничной навигации
        /// </summary>
        /// <returns></returns>
        public async Task<HtmlString> GetPaginationPanelAsync()
        {
            return await Task.Run(async () =>
            {
                TagBuilder nav = new TagBuilder("nav");
                TagBuilder ul = new TagBuilder("ul");

                ul.AddCssClass("pagination form-inline row justify-content-center");
                nav.InnerHtml.AppendHtml(ul);

                TagBuilder liPrev = new TagBuilder("li");
                liPrev.Attributes.Add("class", $"page-item {(_data.IsHavePreviousPage ? "" : "disabled")}");
                var textPrevious = "Предыдущая";
                if (!_data.IsHavePreviousPage)
                {
                    TagBuilder span = new TagBuilder("span");
                    span.Attributes.Add("class", "page-link");
                    span.InnerHtml.Append(textPrevious);
                    liPrev.InnerHtml.AppendHtml(span);
                }
                else
                {
                    TagBuilder a = new TagBuilder("a");
                    a.Attributes.Add("class", "page-link");
                    a.Attributes.Add("href",
                        GetHref(sort: _data.Settings.SortField,
                            sortType: _data.Settings.SortType,
                            page: _data.Settings.CurrentPage - 1,
                            pageSize: _data.Settings.PageRowsCount));
                    a.InnerHtml.Append(textPrevious);
                    liPrev.InnerHtml.AppendHtml(a);
                }

                TagBuilder liNext = new TagBuilder("li");
                liNext.Attributes.Add("class", $"page-item {(_data.IsHaveNextPage ? "" : "disabled")}");
                var textNext = "Следующая";
                if (!_data.IsHaveNextPage)
                {
                    TagBuilder span = new TagBuilder("span");
                    span.Attributes.Add("class", "page-link");
                    span.InnerHtml.Append(textNext);
                    liNext.InnerHtml.AppendHtml(span);
                }
                else
                {
                    TagBuilder a = new TagBuilder("a");
                    a.Attributes.Add("class", "page-link");
                    a.Attributes.Add("href",
                        GetHref(sort: _data.Settings.SortField,
                            sortType: _data.Settings.SortType,
                            page: _data.Settings.CurrentPage + 1,
                            pageSize: _data.Settings.PageRowsCount));
                    a.InnerHtml.Append(textNext);
                    liNext.InnerHtml.AppendHtml(a);
                }

                ul.InnerHtml.AppendHtml(liPrev);

                int currSection = (_data.Settings.CurrentPage - 1) / PAGE_COUNT; //Текущая секция страниц (с ноля)
                for (int i = 1; i <= PAGE_COUNT; i++)
                {
                    int pageNum = i + (currSection * PAGE_COUNT);
                    if (pageNum <= _data.AmountPage) //Если текущая страница входит в общее кол-во страниц
                    {
                        TagBuilder li = new TagBuilder("li");
                        li.Attributes.Add("class", $"page-item {(_data.Settings.CurrentPage == pageNum ? "active" : "")}");
                        ul.InnerHtml.AppendHtml(li);

                        if (_data.Settings.CurrentPage == pageNum)
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.Attributes.Add("class", "page-link");
                            span.InnerHtml.Append(pageNum.ToString());
                            li.InnerHtml.AppendHtml(span);
                        }
                        else
                        {
                            TagBuilder a = new TagBuilder("a");
                            a.Attributes.Add("class", "page-link");
                            a.Attributes.Add("href",
                                GetHref(sort: _data.Settings.SortField,
                                    sortType: _data.Settings.SortType,
                                    page: pageNum,
                                    pageSize: _data.Settings.CurrentPage));

                            a.InnerHtml.Append(pageNum.ToString());
                            li.InnerHtml.AppendHtml(a);
                        }
                    }
                    else break;
                }

                ul.InnerHtml.AppendHtml(liNext);

                return await GetHtmlStringAsync(nav);
            });
        }
        /// <summary>
        /// Получить Html код панели поиска, добавления...
        /// </summary>
        /// <returns></returns>
        public async Task<HtmlString> GetControlPanelAsync(bool isShowBtnAdd = true)
        {
            return await Task.Run(async () =>
            {
                TagBuilder form = new TagBuilder("form");
                form.AddCssClass("form-inline row justify-content-center");
                form.Attributes.Add("method", "get");
                form.Attributes.Add("style", "width: 1000px; margin: auto; margin-bottom: 10px;");

                if (isShowBtnAdd)
                {
                    TagBuilder a = new TagBuilder("a");
                    a.AddCssClass("btn btn-raised btn-success mt-5 mr-3");
                    a.Attributes.Add("href", $"/{_data.ControllerName}/AddUpdateForm");
                    a.InnerHtml.Append("Добавить");
                    form.InnerHtml.AppendHtml(a);
                }

                TagBuilder div = new TagBuilder("div");
                div.AddCssClass("form-group");
                form.InnerHtml.AppendHtml(div);

                TagBuilder input = new TagBuilder("input");
                input.AddCssClass("form-control");
                input.Attributes.Add("type", "text");
                input.Attributes.Add("name", "SearchText");
                input.Attributes.Add("placeholder", "Поиск по всем полям");
                input.Attributes.Add("style", "width: 500px");
                input.Attributes.Add("value", _data.Settings.SearchText);
                div.InnerHtml.AppendHtml(input);

                TagBuilder btn = new TagBuilder("button");
                btn.AddCssClass("btn btn-primary mt-5");
                btn.Attributes.Add("type", "submit");
                btn.InnerHtml.Append("Поиск");
                form.InnerHtml.AppendHtml(btn);

                form.InnerHtml.AppendFormat($"<input type='hidden' name='CurrentPage' value='{_data.Settings.CurrentPage}' />");
                form.InnerHtml.AppendFormat($"<input type='hidden' name='PageRowsCount' value='{_data.Settings.PageRowsCount}' />");
                form.InnerHtml.AppendFormat($"<input type='hidden' name='SortField' value='{_data.Settings.SortField}' />");
                form.InnerHtml.AppendFormat($"<input type='hidden' name='SortType' value='{(int)_data.Settings.SortType}' />");

                return await GetHtmlStringAsync(form);
            });
        }
        private async Task<HtmlString> GetHtmlStringAsync(TagBuilder builder)
        {
            return await Task.Run(() =>
            {
                var writer = new StringWriter();
                builder.WriteTo(writer, HtmlEncoder.Default);
                return new HtmlString(writer.ToString());
            });
        }
        private string GetHref(string sort, SortType sortType, int page, int pageSize)
            => $"/{_data.ControllerName}?SortField={sort}&SorType={(int)sortType}&CurrentPage={page}&PageRowsCount={pageSize}&SearchText={_data.Settings.SearchText}";
    }
}
