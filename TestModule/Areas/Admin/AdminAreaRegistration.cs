using System.Web.Mvc;
using System.Web.Routing;

namespace TestModule.Admin
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
            //    name: "TestModuleAdmin",
            //    url: "admin/testModule/{action}/{id}",
            //    defaults: new { controller = "TestModule", action = "Index", id = UrlParameter.Optional },
            //    namespaces:  new[] { "TestModule.Admin.Controllers" }
            //);
        }
    }
}