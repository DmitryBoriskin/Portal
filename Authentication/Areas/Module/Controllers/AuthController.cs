using Portal.Areas.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Authentication.Module.Controllers
{
    public class AuthController : CoreController
    {
        // GET: Admin/News
        public ActionResult Index()
        {
            return View();
        }
    }
}
