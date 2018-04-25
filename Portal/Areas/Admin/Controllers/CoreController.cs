using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    [Authorize]
    public class CoreController : Controller
    {
        // GET: Admin/Core
        public ActionResult Index2()
        {
            return View();
        }
    }
}