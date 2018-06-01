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