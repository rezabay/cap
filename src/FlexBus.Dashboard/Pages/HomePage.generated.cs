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

namespace FlexBus.Dashboard.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    #line 2 "..\..\Pages\HomePage.cshtml"
    using FlexBus.Dashboard.Pages;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Pages\HomePage.cshtml"
    using FlexBus.Dashboard.Resources;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Pages\HomePage.cshtml"
    using FlexBus.Messages;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Pages\HomePage.cshtml"
    using Newtonsoft.Json;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class HomePage : FlexBus.Dashboard.RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");







            
            #line 7 "..\..\Pages\HomePage.cshtml"
  
    Layout = new LayoutPage(Strings.HomePage_Title);

    var monitor = Storage.GetMonitoringApi();
    var publishedSucceeded = monitor.HourlySucceededJobs(MessageType.Publish);
    var publishedFailed = monitor.HourlyFailedJobs(MessageType.Publish);

    var receivedSucceeded = monitor.HourlySucceededJobs(MessageType.Subscribe);
    var receivedFailed = monitor.HourlyFailedJobs(MessageType.Subscribe);


            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"col-md-12\">\r\n        <h1 class=\"page-header\"" +
">");


            
            #line 20 "..\..\Pages\HomePage.cshtml"
                           Write(Strings.HomePage_Title);

            
            #line default
            #line hidden
WriteLiteral("</h1>\r\n");


            
            #line 21 "..\..\Pages\HomePage.cshtml"
         if (Metrics.Count > 0)
        {

            
            #line default
            #line hidden
WriteLiteral("            <div class=\"row\">\r\n");


            
            #line 24 "..\..\Pages\HomePage.cshtml"
                 foreach (var metric in Metrics)
                {

            
            #line default
            #line hidden
WriteLiteral("                    <div class=\"col-md-2\">\r\n                        ");


            
            #line 27 "..\..\Pages\HomePage.cshtml"
                   Write(Html.BlockMetric(metric));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n");


            
            #line 29 "..\..\Pages\HomePage.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("            </div>\r\n");


            
            #line 31 "..\..\Pages\HomePage.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("        <h3>");


            
            #line 32 "..\..\Pages\HomePage.cshtml"
       Write(Strings.HomePage_RealtimeGraph);

            
            #line default
            #line hidden
WriteLiteral("</h3>\r\n        <div id=\"realtimeGraph\"\r\n             data-published-succeeded=\"");


            
            #line 34 "..\..\Pages\HomePage.cshtml"
                                  Write(Statistics.PublishedSucceeded);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-published-failed=\"");


            
            #line 35 "..\..\Pages\HomePage.cshtml"
                               Write(Statistics.PublishedFailed);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-published-succeeded-string=\"");


            
            #line 36 "..\..\Pages\HomePage.cshtml"
                                         Write(Strings.HomePage_GraphHover_PSucceeded);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-published-failed-string=\"");


            
            #line 37 "..\..\Pages\HomePage.cshtml"
                                      Write(Strings.HomePage_GraphHover_PFailed);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-received-succeeded=\"");


            
            #line 38 "..\..\Pages\HomePage.cshtml"
                                 Write(Statistics.ReceivedSucceeded);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-received-failed=\"");


            
            #line 39 "..\..\Pages\HomePage.cshtml"
                              Write(Statistics.ReceivedFailed);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-received-succeeded-string=\"");


            
            #line 40 "..\..\Pages\HomePage.cshtml"
                                        Write(Strings.HomePage_GraphHover_RSucceeded);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-received-failed-string=\"");


            
            #line 41 "..\..\Pages\HomePage.cshtml"
                                     Write(Strings.HomePage_GraphHover_RFailed);

            
            #line default
            #line hidden
WriteLiteral(@""">
        </div>
        <div style=""display: none;"">
            <span data-metric=""published_succeeded:count""></span>
            <span data-metric=""published_failed:count""></span>
            <span data-metric=""received_succeeded:count""></span>
            <span data-metric=""received_failed:count""></span>
        </div>
        <div id=""legend""></div>
        <h3>
            ");


            
            #line 51 "..\..\Pages\HomePage.cshtml"
       Write(Strings.HomePage_HistoryGraph);

            
            #line default
            #line hidden
WriteLiteral("\r\n        </h3>\r\n\r\n        <div id=\"historyGraph\"\r\n             data-published-su" +
"cceeded=\"");


            
            #line 55 "..\..\Pages\HomePage.cshtml"
                                  Write(JsonConvert.SerializeObject(publishedSucceeded));

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-published-failed=\"");


            
            #line 56 "..\..\Pages\HomePage.cshtml"
                               Write(JsonConvert.SerializeObject(publishedFailed));

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-published-succeeded-string=\"");


            
            #line 57 "..\..\Pages\HomePage.cshtml"
                                         Write(Strings.HomePage_GraphHover_PSucceeded);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-published-failed-string=\"");


            
            #line 58 "..\..\Pages\HomePage.cshtml"
                                      Write(Strings.HomePage_GraphHover_PFailed);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-received-succeeded=\"");


            
            #line 59 "..\..\Pages\HomePage.cshtml"
                                 Write(JsonConvert.SerializeObject(receivedSucceeded));

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-received-failed=\"");


            
            #line 60 "..\..\Pages\HomePage.cshtml"
                              Write(JsonConvert.SerializeObject(receivedFailed));

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-received-succeeded-string=\"");


            
            #line 61 "..\..\Pages\HomePage.cshtml"
                                        Write(Strings.HomePage_GraphHover_RSucceeded);

            
            #line default
            #line hidden
WriteLiteral("\"\r\n             data-received-failed-string=\"");


            
            #line 62 "..\..\Pages\HomePage.cshtml"
                                     Write(Strings.HomePage_GraphHover_RFailed);

            
            #line default
            #line hidden
WriteLiteral("\">\r\n        </div>\r\n    </div>\r\n</div>");


        }
    }
}
#pragma warning restore 1591
