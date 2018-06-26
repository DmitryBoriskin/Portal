using System.Web.Mvc;

namespace VoteModule.Areas.Admin
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
            context.MapRoute(
               "Vote_BE",
               "Admin/Vote/{action}/{id}",
               new { controller = "Vote", action = "Index", id = UrlParameter.Optional },
               new[] { "VoteModule.Areas.Admin.Controllers" }
            );
        }
    }
}