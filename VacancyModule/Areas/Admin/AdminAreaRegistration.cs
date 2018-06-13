using System.Web.Mvc;

namespace VacancyModule.Areas.Admin
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
               "Vacancy_BE",
               "Admin/Vacancy/{action}/{id}",
               new { controller = "Vacancy", action = "Index", id = UrlParameter.Optional },
               new[] { "VacancyModule.Areas.Admin.Controllers" }
            );
        }
    }
}