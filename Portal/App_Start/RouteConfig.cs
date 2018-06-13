using System;
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
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Main", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Portal.Controllers" }
            );
          
        }
    }
}
