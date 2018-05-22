using System.Web.Mvc;

namespace AuthModule.Auth
{
    public class ModuleAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Auth";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
               "Auth_account",
               "Account/{action}/{id}",
               new { controller = "Account", action = "Login", id = UrlParameter.Optional },
               new[] { "AuthModule.Auth.Controllers" }
           );
            context.MapRoute(
               "Auth_manage",
               "AccountManage/{action}/{id}",
               new { controller = "AccountManage", action = "Index", id = UrlParameter.Optional },
               new[] { "AuthModule.Auth.Controllers" }
           );
            context.MapRoute(
              "Auth_home",
              "AccountHome/{action}/{id}",
              new { controller = "AccountHome", action = "Index", id = UrlParameter.Optional },
              new[] { "AuthModule.Auth.Controllers" }
          );
        }
    }
}