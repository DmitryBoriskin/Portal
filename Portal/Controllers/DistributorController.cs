using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class DistributorController : LayoutController
    {
        // GET: Distributor
        public ActionResult Index()
        {

            //model.Page = _Repository.GetPage(_path, _alias);

            return View();
        }
    }
}