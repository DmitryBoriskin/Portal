using System.Web.Mvc;

namespace EventsModule.Areas.Admin
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
              "Events_BE",
              "Admin/Events/{action}/{id}",
              new { controller = "Events", action = "Index", id = UrlParameter.Optional },
              new[] { "EventsModule.Areas.Admin.Controllers" }
           );

            context.MapRoute(
                "EventsWidget_BE",
                "Admin/EventsWidget/{action}/{id}",
                new { controller = "EventsWidget", action = "Index", id = UrlParameter.Optional },
                new[] { "EventsModule.Areas.Admin.Controllers" }
             );
        }
    }
}