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
                namespaces: new[] { "Portal.Controllers" },
                constraints: new { controller = "Admin" }
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
