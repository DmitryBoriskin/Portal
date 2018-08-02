
using System.Web.Mvc;

namespace CartModule.Areas.Admin
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
            //    "Admin_default",
            //    "Admin/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
             "Cart_BE",
             "Admin/Cart/{action}/{id}",
             new { controller = "Cart", action = "Index", id = UrlParameter.Optional },
             new[] { "CartModule.Areas.Admin.Controllers" }
          );
            context.MapRoute(
            "CartCategories_BE",
            "Admin/CartCategories/{action}/{id}",
            new { controller = "CartCategories", action = "Index", id = UrlParameter.Optional },
            new[] { "CartModule.Areas.Admin.Controllers" }
         );
            context.MapRoute(
            "CartProducts_BE",
            "Admin/CartProducts/{action}/{id}",
            new { controller = "CartProducts", action = "Index", id = UrlParameter.Optional },
            new[] { "CartModule.Areas.Admin.Controllers" }
         );
        }
    }
}