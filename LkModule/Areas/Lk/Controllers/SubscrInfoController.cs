using LkModule.Areas.Lk.Models;
using Portal.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LkModule.Areas.Lk.Controllers
{
    [Authorize]
    public class SubscrInfoController : LayoutController
    {
        private SubscrFrontModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new SubscrFrontModel()
            {
                LayoutInfo = _layoutData,
                PageName = _pageName,
                User = CurrentUser,
                Breadcrumbs = _breadcrumb
            };
        }


        public ActionResult Index()
        {
            var userId = CurrentUser.UserId;

            model.Item = _Repository.GetUserSubscrDefault(userId);
            return View(model);
        }
    }
}