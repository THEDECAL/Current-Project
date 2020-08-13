using EasyBilling.Helpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EasyBilling.HtmlHelpers
{
    public class TableHtmlHelper<T> where T : class
    {
        private DataViewModel<T> _data;
        public TableHtmlHelper(DataViewModel<T> data)
        {
            _data = data;
        }
        public HtmlString GetTableHead()
        {
            TagBuilder thead = new TagBuilder("thead");
            TagBuilder tr = new TagBuilder("tr");
            thead.InnerHtml.AppendHtml(tr);

            var type = typeof(T);
            var props = type.GetProperties();
            foreach (var item in props)
            {
                var dnAtt = item.GetCustomAttribute(
                    typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                TagBuilder th = new TagBuilder("th");

                TagBuilder a = new TagBuilder("a");
                th.Attributes.Add("scope", "col");
                a.Attributes.Add("href",
                    $"/{type.Name}s?sort={item.Name}&sortype={(int)_data.SortType}&page={_data.Page}&pageSize={_data.PageSize}");
                a.InnerHtml.Append((dnAtt != null) ? dnAtt.DisplayName : item.Name);
                th.InnerHtml.AppendHtml(a);
                tr.InnerHtml.AppendHtml(th);
            }

            //var writer = new StringWriter();
            //thead.WriteTo(writer, HtmlEncoder.Default);
            //return new HtmlString(writer.ToString());
            return GetHtmlString(thead);
        }
        public HtmlString GetPaginationPanel()
        {
            //< nav aria - label = "..." >
            //   < ul class="pagination">
            //    <li class="page-item disabled">
            //      <span class="page-link">Previous</span>
            //    </li>
            //    <li class="page-item"><a class="page-link" href="#">1</a></li>
            //    <li class="page-item active">
            //      <span class="page-link">
            //        2
            //        <span class="sr-only">(current)</span>
            //      </span>
            //    </li>
            //    <li class="page-item"><a class="page-link" href="#">3</a></li>
            //    <li class="page-item">
            //      <a class="page-link" href="#">Next</a>
            //    </li>
            //  </ul>
            //</nav>

            TagBuilder nav = new TagBuilder("nav");
            TagBuilder ul = new TagBuilder("ul");

            ul.Attributes.Add("class", "pagination");
            nav.InnerHtml.AppendHtml(ul);

            TagBuilder liPrev = new TagBuilder("li");
            liPrev.Attributes.Add("class", $"page-item {(_data.IsHavePreviousPage ? "" : "disabled")}");
            liPrev.InnerHtml.Append("<span class='page-link'><<<</span>");
            TagBuilder liNext = new TagBuilder("li");
            liNext.Attributes.Add("class", $"page-item {(_data.IsHaveNextPage ? "" : "disabled")}");
            liNext.InnerHtml.Append("<span class='page-link'>>>></span>");

            ul.InnerHtml.AppendHtml(liPrev);



            ul.InnerHtml.AppendHtml(liNext);

            return GetHtmlString(nav);
        }
        private HtmlString GetHtmlString(TagBuilder builder)
        {
            var writer = new StringWriter();
            builder.WriteTo(writer, HtmlEncoder.Default);
            return new HtmlString(writer.ToString());
        }
    }
}
