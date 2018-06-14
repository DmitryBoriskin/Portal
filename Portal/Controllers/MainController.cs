using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class MainController : LayoutController
    {
        private MainFrontViewModel model;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new MainFrontViewModel
            {
                LayutInfo=_layoutmodel                
            };
        }
        // GET: Main
        public ActionResult Index()
        {
            return View(model);
        }
    }
}