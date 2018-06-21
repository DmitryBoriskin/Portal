using Microsoft.AspNet.Identity;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class InfoController : LayoutController
    {
        private InfoFrontModel model;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new InfoFrontModel()
            {
                LayoutInfo = _layoutData,
                PageName = _pageName,
                User = CurrentUser,
                Breadcrumbs = _breadcrumb
            };
        }
        // GET: Info
        public ActionResult Index()
        {
            if (model.LayoutInfo.DefaultSubscr != null)
            {
             //   model.DefaultSubscrInfo = _Repository.GetUserSubscrDefault(Guid.Parse(User.Identity.GetUserId()));
                return View(model);
            }
            else return Redirect("/settings");
        }
    }
}