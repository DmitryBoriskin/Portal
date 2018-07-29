using PgDbase.entity;
using PgDbase.Repository.front;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventsModule.Areas.Events.Controllers
{
    public class EventsController : CoreController
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

        // GET: Events/Index
        public ActionResult Index()
        {
            FilterModel filter = GetFilter();
            var model = _Repository.GetEvents(filter);
            return View(model);
        }

        public ActionResult Item(int id)
        {
            var model = _Repository.GetEventItem(id);
            return View(model);
        }


    }
}