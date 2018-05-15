using System.Web.Mvc;
using System.Web.Routing;

namespace TestModule.Module
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
               name: "TestModule",
               url: "testModule/{action}/{id}",
               defaults: new { controller = "TestModule", action = "Index", id = UrlParameter.Optional },
               namespaces: new[] { "TestModule.Module.Controllers" }
           );
        }
    }
}