﻿using PgDbase.entity;
using Portal.Controllers;
using Portal.Models;
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

        // GET: Vacancy/Item/<:id>
        public ActionResult Item(Guid id)
        {
            var model = _Repository.GetVacancy(id);
            return View(model);
        }

        // GET: Vacancy/Widget/<:viewName>
        public ActionResult Widget(WidgetParamHelper helper)
        {
            VacancyWidgetModel model = new VacancyWidgetModel
            {
                Title = helper.Title,
                List = _Repository.GetVacancies(helper.Count)
            };

            return View(helper.ViewName, model);
        }
    }
}