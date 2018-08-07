using PgDbase.entity;
using PgDbase.Repository.front;
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
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");
        }

        // GET: Vacancy/Vacancy
        public ActionResult Index()
        {
            FilterModel filter = GetFilter();
            var model = _Repository.GetVacancies(filter);
            return View(ViewName,model);
        }

        // GET: Vacancy/Item/<:id>
        public ActionResult Item(Guid id)
        {
            var model = _Repository.GetVacancy(id);
            return View(ViewName,model);
        }

#pragma warning disable CS1030 // #warning: 'Исправить - не нужно передавать в параметрах viewname'
#warning Исправить - не нужно передавать в параметрах viewname
        // GET: Vacancy/Widget/<:viewName>
        public ActionResult Widget(WidgetParamHelper helper)
#pragma warning restore CS1030 // #warning: 'Исправить - не нужно передавать в параметрах viewname'
        {
            VacancyWidgetModel model = new VacancyWidgetModel
            {
                Title = helper.Title,
                List = _Repository.GetVacancies(helper.Count)
            };

            return View(ViewName, model);
        }
    }
}