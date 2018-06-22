using System.Web.Mvc;

namespace MessagesModule.Areas.Messages
{
    public class MessagesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Messages";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

            context.MapRoute(
                 "Messages_FE",
                 "Messages/{action}/{id}",
                 new { controller = "Messages", action = "Index", id = UrlParameter.Optional },
                 new[] { "MessagesModule.Areas.Messages.Controllers" }                 
             );

            context.MapRoute(
                 "Messages_widget_FE",
                 "MsgWidget/{action}/{id}",
                 new { controller = "MsgWidget", action = "Index", id = UrlParameter.Optional },
                 new[] { "MessagesModule.Areas.Messages.Controllers" }
             );


        }
    }
}