using System.Web.Mvc;

namespace EventsModule.Areas.Events
{
    public class EventsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Events";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

           

            context.MapRoute(
                "Events_default",
                "Events/{action}/{id}",
                new { controller="events",action = "Index", id = UrlParameter.Optional },
                new[] { "EventsModule.Areas.Events.Controllers" }
            );
            context.MapRoute(
             "EventsItem_default",
             "Events/{id}",
             new { controller = "events", action = "Item", id = UrlParameter.Optional },
             new[] { "EventsModule.Areas.Events.Controllers" }
           );
        }
    }
}