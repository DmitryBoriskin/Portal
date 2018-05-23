using System.Web.Mvc;

namespace VacancyModule.Areas.Vacancy
{
    public class VacancyAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Vacancy";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Vacancy_default",
                "Vacancy/{action}/{id}",
                new { controller = "Vacancy", action = "Index", id = UrlParameter.Optional },
                new[] { "VacancyModule.Areas.Vacancy.Controllers" }
            );
        }
    }
}