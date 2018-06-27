using System.Web.Mvc;

namespace VoteModule.Areas.Vote
{
    public class VoteAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Vote";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

            context.MapRoute(
                 "Vote_FE",
                 "Vote/{action}/{id}",
                 new { controller = "Vote", action = "Index", id = UrlParameter.Optional },
                 new[] { "VoteModule.Areas.Vote.Controllers" }
             );

            context.MapRoute(
                 "Vote_widget_FE",
                 "VoteWidget/{action}/{id}",
                 new { controller = "VoteWidget", action = "Index", id = UrlParameter.Optional },
                 new[] { "VoteModule.Areas.Vote.Controllers" }
             );


        }
    }
}