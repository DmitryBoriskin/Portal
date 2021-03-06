﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Portal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //типовая страница
            routes.MapRoute(
                name: "Page",
                url: "page/{*path}",
                defaults: new { controller = "Page", action = "Index", path = UrlParameter.Optional },
                namespaces: new[] { "Portal.Controllers" }
            );


            routes.MapRoute(
                name: "News",
                url: "news/",
                defaults: new { controller = "News", action = "Index", path = UrlParameter.Optional },
                namespaces: new[] { "Portal.Controllers" }
            );

            routes.MapRoute(
                name: "NewsItem",
                url: "news/{*path}",
                defaults: new { controller = "News", action = "NewsItem", path = UrlParameter.Optional },
                namespaces: new[] { "Portal.Controllers" }
            );

            routes.MapRoute(
                name: "Rights",
                url: "rights/",
                defaults: new { controller = "Rights", action = "Index", path = UrlParameter.Optional },
                namespaces: new[] { "Portal.Controllers" }
            );

            routes.MapRoute(
                name: "RightsItem",
                url: "rights/{*path}",
                defaults: new { controller = "Rights", action = "NewsItem", path = UrlParameter.Optional },
                namespaces: new[] { "Portal.Controllers" }
            );

            routes.MapRoute(
                name: "PageGroup",
                url: "Widget/PageGroup",
                defaults: new { controller = "Widget", action = "PageGroup", path = UrlParameter.Optional },
                namespaces: new[] { "Portal.Controllers" }
            );
            //   routes.MapRoute(
            //    name: "Default2",
            //    url: "{*path}",
            //    defaults: new { controller = "Distributor", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "Portal.Controllers" }
            //);
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Main", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Portal.Controllers" }
            );
         
          
        }
    }
}
