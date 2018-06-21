using System.Web.Mvc;

namespace MessagesModule.Areas.Admin
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
                  "Msg_BE",
                  "Admin/Messages/{action}/{id}",
                  new { controller = "Messages", action = "Index", id = UrlParameter.Optional },
                  new[] { "MessagesModule.Areas.Admin.Controllers" }
               );


        }
    }
}