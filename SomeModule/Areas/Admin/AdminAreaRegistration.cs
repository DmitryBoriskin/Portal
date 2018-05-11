using System.Web.Mvc;
using System.Web.Routing;

namespace SomeModule.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: "SomeModuleAdmin",
                url: "Admin/SomeModule/{action}/{id}",
                defaults: new { controller = "SomeModule", action = "Index", id = UrlParameter.Optional },
                namespaces:  new[] { "SomeModule.Admin.Controllers" }
            );

           // context.MapRoute(
           //    name: "SomeModule",
           //    url: "SomeModule/{action}/{id}",
           //    defaults: new { controller = "SomeModule", action = "Index", id = UrlParameter.Optional },
           //    namespaces: new[] { "SomeModule.Controllers" }
           //);
        }
    }
}