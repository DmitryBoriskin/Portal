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
                "Events_FE",
                "Events/{action}/{id}",
                new { controller="events",action = "Index", id = UrlParameter.Optional },
                new[] { "EventsModule.Areas.Events.Controllers" }
            );
        }
    }
}