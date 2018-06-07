using PgDbase.Repository.front;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class PageController : CoreController
    {
        private PageWidgetModel modelwidget;
        // GET: Page
        public ActionResult Index()
        {
            var test = "sgs";
            return View();
        }
        public ActionResult Group(string alias)
        {
            //modelwidget.PageGroup=_Repository.GetPageChilds()
            return View(modelwidget);
        }
    }
}