using System.Web.Mvc;

namespace AuthModule.Areas.Admin
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
                "Auth_Admin",
                "Admin/Auth/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "AuthModule.Admin.Controllers" }
            );
        }
    }
}