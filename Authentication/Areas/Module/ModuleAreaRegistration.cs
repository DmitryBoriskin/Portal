using System.Web.Mvc;
using System.Web.Routing;

namespace Authentication.Module
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
                       name: "Auth",
                       url: "auth/{action}/{id}",
                       defaults: new { controller = "Auth", action = "Index", id = UrlParameter.Optional },
                       namespaces: new[] { "Authentication.Module.Controllers" }
                   );
        }
    }
}