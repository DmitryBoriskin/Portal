using System.Web.Mvc;

namespace Authentication.Admin
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
            //    name: "AuthAdmin",
            //    url: "admin/auth/{action}/{id}",
            //    defaults: new { controller = "Auth", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "Authentication.Admin.Controllers" }
            //);
        }
    }
}