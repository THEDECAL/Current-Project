#pragma checksum "E:\Документы\Visual Studio 2017\Projects\CSharp\ASP.NET\online_checkers\online_checkers\Views\Game\ListGames.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "bdc3f2cd3bdcaf6ed8909ebed264a3e4689c9178"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Game_ListGames), @"mvc.1.0.view", @"/Views/Game/ListGames.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Game/ListGames.cshtml", typeof(AspNetCore.Views_Game_ListGames))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "E:\Документы\Visual Studio 2017\Projects\CSharp\ASP.NET\online_checkers\online_checkers\Views\_ViewImports.cshtml"
using online_checkers;

#line default
#line hidden
#line 1 "E:\Документы\Visual Studio 2017\Projects\CSharp\ASP.NET\online_checkers\online_checkers\Views\Game\ListGames.cshtml"
using online_checkers.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"bdc3f2cd3bdcaf6ed8909ebed264a3e4689c9178", @"/Views/Game/ListGames.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1615fb965999f61719b180a2db57bee3c1dc5f2f", @"/Views/_ViewImports.cshtml")]
    public class Views_Game_ListGames : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<List<Game>>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 3 "E:\Документы\Visual Studio 2017\Projects\CSharp\ASP.NET\online_checkers\online_checkers\Views\Game\ListGames.cshtml"
  
    ViewData["Title"] = "Список игр";

#line default
#line hidden
            BeginContext(96, 6, true);
            WriteLiteral("\r\n<h2>");
            EndContext();
            BeginContext(103, 17, false);
#line 7 "E:\Документы\Visual Studio 2017\Projects\CSharp\ASP.NET\online_checkers\online_checkers\Views\Game\ListGames.cshtml"
Write(ViewData["Title"]);

#line default
#line hidden
            EndContext();
            BeginContext(120, 35, true);
            WriteLiteral("</h2>\r\n\r\n<div class=\"list-games\">\r\n");
            EndContext();
#line 10 "E:\Документы\Visual Studio 2017\Projects\CSharp\ASP.NET\online_checkers\online_checkers\Views\Game\ListGames.cshtml"
     foreach (var item in Model)
    {

    }

#line default
#line hidden
            BeginContext(205, 8, true);
            WriteLiteral("</div>\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<List<Game>> Html { get; private set; }
    }
}
#pragma warning restore 1591
