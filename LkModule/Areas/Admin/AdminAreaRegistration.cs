using System.Web.Mvc;

namespace LkModule.Areas.Admin
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
              "Subscrs_BE",
              "Admin/Subscrs/{action}/{id}",
              new { controller = "Subscrs", action = "Index", id = UrlParameter.Optional },
              new[] { "LkModule.Areas.Admin.Controllers" }
           );
            context.MapRoute(
            "SubscrsWidget_BE",
            "Admin/SubscrsWidget/{action}/{id}",
            new { controller = "SubscrsWidget", action = "Index", id = UrlParameter.Optional },
            new[] { "LkModule.Areas.Admin.Controllers" }
         );
            context.MapRoute(
            "Accruals_BE",
            "Admin/Accruals/{action}/{id}",
            new { controller = "Accruals", action = "Index", id = UrlParameter.Optional },
            new[] { "LkModule.Areas.Admin.Controllers" }
         );
            context.MapRoute(
              "Departments_BE",
              "Admin/Departments/{action}/{id}",
              new { controller = "Departments", action = "Index", id = UrlParameter.Optional },
              new[] { "LkModule.Areas.Admin.Controllers" }
           );
            context.MapRoute(
              "PU_BE",
              "Admin/PU/{action}/{id}",
              new { controller = "PU", action = "Index", id = UrlParameter.Optional },
              new[] { "LkModule.Areas.Admin.Controllers" }
           );
            context.MapRoute(
             "Payments_BE",
             "Admin/Payments/{action}/{id}",
             new { controller = "Payments", action = "Index", id = UrlParameter.Optional },
             new[] { "LkModule.Areas.Admin.Controllers" }
          );
        }
    }
}