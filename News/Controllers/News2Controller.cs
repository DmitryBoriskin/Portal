using PgDbase;
using PgDbase.entity;
using PgDbase.Repository.cms;
using Portal.Areas.Admin;
using Portal.Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;


namespace Portal.Areas.Admin.Controllers //News
{
    public class News2Controller: CoreController
    {
        // GET: Admin/News
        public ActionResult Index()
        {
            return View();
        }
    }
}
