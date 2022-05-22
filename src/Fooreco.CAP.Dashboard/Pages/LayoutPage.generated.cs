﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Fooreco.CAP.Dashboard.Pages
{
    
    #line 2 "..\..\Pages\LayoutPage.cshtml"
    using System;
    
    #line default
    #line hidden
    using System.Collections.Generic;
    
    #line 3 "..\..\Pages\LayoutPage.cshtml"
    using System.Globalization;
    
    #line default
    #line hidden
    using System.Linq;
    
    #line 4 "..\..\Pages\LayoutPage.cshtml"
    using System.Reflection;
    
    #line default
    #line hidden
    using System.Text;
    
    #line 5 "..\..\Pages\LayoutPage.cshtml"
    using Fooreco.CAP.Dashboard.Pages;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Pages\LayoutPage.cshtml"
    using Fooreco.CAP.Dashboard.Resources;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    public partial class LayoutPage : Fooreco.CAP.Dashboard.RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");







WriteLiteral("<!DOCTYPE html>\r\n<html lang=\"");


            
            #line 9 "..\..\Pages\LayoutPage.cshtml"
       Write(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n<head>\r\n    <title>");


            
            #line 11 "..\..\Pages\LayoutPage.cshtml"
      Write(Title);

            
            #line default
            #line hidden
WriteLiteral(" - CAP</title>\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n    <m" +
"eta charset=\"utf-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, ini" +
"tial-scale=1.0\">\r\n");


            
            #line 15 "..\..\Pages\LayoutPage.cshtml"
       var version = GetType().GetTypeInfo().Assembly.GetName().Version; 

            
            #line default
            #line hidden
WriteLiteral("    <link rel=\"stylesheet\" href=\"");


            
            #line 16 "..\..\Pages\LayoutPage.cshtml"
                            Write(Url.To($"/css{version.Major}{version.Minor}{version.Build}"));

            
            #line default
            #line hidden
WriteLiteral(@""">
</head>
<body>
<!-- Wrap all page content here -->
<div id=""wrap"">

    <!-- Fixed navbar -->
    <div class=""navbar navbar-default navbar-fixed-top"">
        <div class=""container"">
            <div class=""navbar-header"">
                <button type=""button"" class=""navbar-toggle"" data-toggle=""collapse"" data-target="".navbar-collapse"">
                    <span class=""icon-bar""></span>
                    <span class=""icon-bar""></span>
                    <span class=""icon-bar""></span>
                </button>
                <a class=""navbar-brand"" href=""");


            
            #line 31 "..\..\Pages\LayoutPage.cshtml"
                                         Write(Url.Home());

            
            #line default
            #line hidden
WriteLiteral("\">CAP Dashboard</a>\r\n            </div>\r\n            <div class=\"collapse navbar-" +
"collapse\">\r\n                ");


            
            #line 34 "..\..\Pages\LayoutPage.cshtml"
           Write(Html.RenderPartial(new Navigation()));

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 35 "..\..\Pages\LayoutPage.cshtml"
                 if (AppPath != null)
                {

            
            #line default
            #line hidden
WriteLiteral("                    <ul class=\"nav navbar-nav navbar-right\">\r\n                   " +
"     <li>\r\n                            <a href=\"");


            
            #line 39 "..\..\Pages\LayoutPage.cshtml"
                                Write(AppPath);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                                <span class=\"glyphicon glyphicon-log-out\"></s" +
"pan>\r\n                                ");


            
            #line 41 "..\..\Pages\LayoutPage.cshtml"
                           Write(Strings.LayoutPage_Back);

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </a>\r\n                        </li>\r\n              " +
"      </ul>\r\n");


            
            #line 45 "..\..\Pages\LayoutPage.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("            </div>\r\n            <!--/.nav-collapse -->\r\n        </div>\r\n    </div" +
">\r\n\r\n    <!-- Begin page content -->\r\n    <div class=\"container\" style=\"margin-b" +
"ottom: 20px;\">\r\n        ");


            
            #line 53 "..\..\Pages\LayoutPage.cshtml"
   Write(RenderBody());

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>\r\n\r\n<div id=\"footer\">\r\n    <div class=\"container\">\r\n        <" +
"ul class=\"list-inline credit\">\r\n            <li>\r\n                <a href=\"https" +
"://github.com/dotnetcore/cap/\" target=\"_blank\">\r\n                    CAP ");


            
            #line 62 "..\..\Pages\LayoutPage.cshtml"
                    Write($"{version.Major}.{version.Minor}.{version.Build}");

            
            #line default
            #line hidden
WriteLiteral("\r\n                </a>\r\n            </li>\r\n            <li>");


            
            #line 65 "..\..\Pages\LayoutPage.cshtml"
           Write(Storage);

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n            <li>");


            
            #line 66 "..\..\Pages\LayoutPage.cshtml"
           Write(Strings.LayoutPage_Footer_Time);

            
            #line default
            #line hidden
WriteLiteral(" ");


            
            #line 66 "..\..\Pages\LayoutPage.cshtml"
                                           Write(Html.LocalTime(DateTime.UtcNow));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n            <li>");


            
            #line 67 "..\..\Pages\LayoutPage.cshtml"
           Write(string.Format(Strings.LayoutPage_Footer_Generatedms, GenerationTime.Elapsed.TotalMilliseconds.ToString("N")));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");


            
            #line 68 "..\..\Pages\LayoutPage.cshtml"
             if (NodeName != null)
            {

            
            #line default
            #line hidden
WriteLiteral("                <li>");


            
            #line 70 "..\..\Pages\LayoutPage.cshtml"
               Write(string.Format(Strings.LayoutPage_Footer_NodeCurrent, NodeName));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");


            
            #line 71 "..\..\Pages\LayoutPage.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("        </ul>\r\n    </div>\r\n</div>\r\n\r\n<div id=\"capConfig\"\r\n     data-pollinterval=" +
"\"");


            
            #line 77 "..\..\Pages\LayoutPage.cshtml"
                   Write(StatsPollingInterval);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n     data-pollurl=\"");


            
            #line 78 "..\..\Pages\LayoutPage.cshtml"
               Write(Url.To("/stats"));

            
            #line default
            #line hidden
WriteLiteral("\">\r\n</div>\r\n\r\n<script src=\"");


            
            #line 81 "..\..\Pages\LayoutPage.cshtml"
        Write(Url.To($"/js{version.Major}{version.Minor}{version.Build}"));

            
            #line default
            #line hidden
WriteLiteral("\"></script>\r\n</body>\r\n</html>");


        }
    }
}
#pragma warning restore 1591
