using System.Web.Mvc;

namespace LkModule.Areas.Lk
{
    public class LkAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Lk";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //context.MapRoute(
            //    "Lk_default",
            //    "Lk/{controller}/{action}/{id}",
            //    new { action = "Index", id = UrlParameter.Optional }
            //);

            context.MapRoute(
                "Invoices_FE",
                "Lk/Invoices/{action}/{id}",
                new { controller = "Invoices", action = "Index", id = UrlParameter.Optional },
                new[] { "LkModule.Areas.Lk.Controllers" }
            );
            context.MapRoute(
                "Payments_FE",
                "Lk/Payments/{action}/{id}",
                new { controller = "Payments", action = "Index", id = UrlParameter.Optional },
                new[] { "LkModule.Areas.Lk.Controllers" }
                );
            context.MapRoute(
                "Pu_FE",
                "Lk/Pu/{action}/{id}",
                new { controller = "Pu", action = "Index", id = UrlParameter.Optional },
                new[] { "LkModule.Areas.Lk.Controllers" }
                );
            context.MapRoute(
             "SubscrInfo_FE",
             "Lk/SubscrInfo/{action}/{id}",
             new { controller = "SubscrInfo", action = "Index", id = UrlParameter.Optional },
             new[] { "LkModule.Areas.Lk.Controllers" }
             );
            context.MapRoute(
             "Statistics_FE",
             "Lk/Statistics/{action}/{id}",
             new { controller = "Statistics", action = "Index", id = UrlParameter.Optional },
             new[] { "LkModule.Areas.Lk.Controllers" }
             );
            context.MapRoute(
              "LkWidgets_FE",
              "Lk/LkWidgets/{action}/{id}",
              new { controller = "LkWidgets", action = "Info", id = UrlParameter.Optional },
              new[] { "LkModule.Areas.Lk.Controllers" }
              );
        }
    }
}