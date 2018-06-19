﻿using Microsoft.AspNet.Identity;
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
                LayoutInfo = _layoutmodel,
                User = _user,
                Breadcrumbs= _breadcrumb
            };
        }
        // GET: Info
        public ActionResult Index()
        {
            if (model.LayoutInfo.DefaultSubscr != null)
            {
                model.DefaultSubscrInfo=_Repository.GetInfoSubscrDefault(Guid.Parse(User.Identity.GetUserId()));
                return View(model);
            }
            else return  Redirect("/settings");            
        }
    }
}