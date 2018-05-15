using System.Web.Mvc;
using System.Web.Routing;

namespace SomeModule.Module
{
    public class ModuleRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Module";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                           name: "SomeModule",
                           url: "someModule/{action}/{id}",
                           defaults: new { controller = "SomeModule", action = "Index", id = UrlParameter.Optional },
                           namespaces: new[] { "SomeModule.Module.Controllers" }
                       );
        }
    }
}