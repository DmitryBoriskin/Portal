using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class MainController : CoreController
    {
        // GET: Admin/Main
        public ActionResult Index()
        {
            return View();
        }
    }
}