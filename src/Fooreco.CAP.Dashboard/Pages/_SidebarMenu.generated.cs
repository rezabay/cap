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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class SidebarMenu : Fooreco.CAP.Dashboard.RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");



            
            #line 3 "..\..\Pages\_SidebarMenu.cshtml"
 if (Items.Any())
{

            
            #line default
            #line hidden
WriteLiteral("    <div id=\"stats\" class=\"list-group\">\r\n");


            
            #line 6 "..\..\Pages\_SidebarMenu.cshtml"
         foreach (var item in Items)
        {
            var itemValue = item(this);

            
            #line default
            #line hidden
WriteLiteral("            <a href=\"");


            
            #line 9 "..\..\Pages\_SidebarMenu.cshtml"
                Write(itemValue.Url);

            
            #line default
            #line hidden
WriteLiteral("\" class=\"list-group-item ");


            
            #line 9 "..\..\Pages\_SidebarMenu.cshtml"
                                                        Write(itemValue.Active ? "active" : null);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n                ");


            
            #line 10 "..\..\Pages\_SidebarMenu.cshtml"
           Write(itemValue.Text);

            
            #line default
            #line hidden
WriteLiteral("\r\n                <span class=\"pull-right\">\r\n");


            
            #line 12 "..\..\Pages\_SidebarMenu.cshtml"
                     foreach (var metric in itemValue.GetAllMetrics())
                    {
                        
            
            #line default
            #line hidden
            
            #line 14 "..\..\Pages\_SidebarMenu.cshtml"
                   Write(Html.InlineMetric(metric));

            
            #line default
            #line hidden
            
            #line 14 "..\..\Pages\_SidebarMenu.cshtml"
                                                  
                    }

            
            #line default
            #line hidden
WriteLiteral("                </span>\r\n            </a>\r\n");


            
            #line 18 "..\..\Pages\_SidebarMenu.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n");


            
            #line 20 "..\..\Pages\_SidebarMenu.cshtml"
}
            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591