using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SomeModule.Module.Controllers
{
    public class SomeModuleController: Controller
    {
        // GET: Admin/News
        public ActionResult Index()
        {
            return View();
        }
    }
}
