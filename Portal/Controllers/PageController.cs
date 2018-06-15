using PgDbase.Repository.front;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class PageController : LayoutController
    {
        private PageFrontModel model;
        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new PageFrontModel() {
                LayoutInfo = _layoutmodel,
                User = _user
            };       
        }

        // GET: Page
        public ActionResult Index()
        { 
            model.Page = _Repository.GetPage(_path,_alias);
            return View(model);
        }
        public ActionResult Group(string alias,string view)
        {
            model.PageGroup = _Repository.GetPageGroup(alias);

            if (String.IsNullOrEmpty(view))
            {
                return View(model);
            }
            else return View(view,model);

        }
        public ActionResult Child()
        {
            model.PageGroup = _Repository.GetPageChild(_path,_alias);
            return View("Group",model);
        }
    }
}