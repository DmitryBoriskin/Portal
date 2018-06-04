using Portal.Areas.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("Departments")]
    public class DepartmentsController : BeCoreController
    {
        // GET: Admin/Department
        public ActionResult Index()
        {
            return View();
        }
    }
}