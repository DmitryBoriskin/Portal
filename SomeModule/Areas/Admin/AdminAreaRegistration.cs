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
            //context.MapRoute(
            //                name: "SomeModuleAdmin",
            //                url: "admin/someModule/{action}/{id}",
            //                defaults: new { controller = "SomeModule", action = "Index", id = UrlParameter.Optional },
            //                namespaces:  new[] { "SomeModule.Admin.Controllers" }
            //            );
        }
    }
}