using PgDbase.entity;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VacancyModule.Areas.Vacancy.Controllers
{
    public class VacancyController : CoreController
    {
        // GET: Vacancy/Vacancy
        public ActionResult Index()
        {
            FilterModel filter = GetFilter();
            var model = _Repository.GetVacancies(filter);
            return View(model);
        }
    }
}