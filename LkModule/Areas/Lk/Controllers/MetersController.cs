﻿using LkModule.Areas.Lk.Models;
using PgDbase.entity;
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
    public class MetersController : LayoutController
    {
        FilterModel filter;
        PuFrontModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            model = new PuFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };
        }


        public ActionResult Index()
        {
            filter = GetFilter();

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);

            if (userSubscr != null)
            {
                var pFilter = FilterModel.Extend<LkFilter>(filter);
                model.List = _Repository.GetSubscrDevices(userSubscr.Id, pFilter);
            }

            if (filter.Date.HasValue)
                ViewBag.beginDate = filter.Date.Value.ToString("dd.MM.yyyy");

            if (filter.DateEnd.HasValue)
                ViewBag.endDate = filter.DateEnd.Value.ToString("dd.MM.yyyy");

            return View(ViewName, model);
        }
    }
}