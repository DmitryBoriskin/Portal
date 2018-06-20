using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageModule.Areas.Admin.Contrllers
{
    public class MessagesController : Controller
    {
        // GET: Admin/Messages
        public ActionResult Index()
        {
            return View();
        }
    }
}