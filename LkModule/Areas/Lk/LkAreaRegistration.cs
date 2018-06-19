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
            "Accruals_FE",
            "Lk/Accruals/{action}/{id}",
            new { controller = "Accruals", action = "Index", id = UrlParameter.Optional },
            new[] { "LkModule.Areas.Lk.Controllers" }
        );
        context.MapRoute(
            "Payments_FE",
            "Lk/Payments/{action}/{id}",
            new { controller = "Payments", action = "Index", id = UrlParameter.Optional },
            new[] { "LkModule.Areas.Lk.Controllers" }
            );
        }
    }
}