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
    public class PaymentsController : LayoutController
    {
        FilterModel filter;
        PaymentFrontModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/Page/ModuleDenied");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            model = new PaymentFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };
        }

        // GET: Admin/Payments
        public ActionResult Index()
        {
            filter = GetFilter();

            var userId = CurrentUser.UserId;

            var mFilter = FilterModel.Extend<LkFilter>(filter);
            mFilter.Status = ViewBag.Status = Request.Params["status"];
            mFilter.Type = ViewBag.Type = Request.Params["type"];

            var userSubscr = _Repository.GetUserSubscrDefault(userId);

            if (userSubscr != null)
            {
                model.List = _Repository.GetPayments(userSubscr.Id, mFilter);
            }

            if (mFilter.Date.HasValue)
                ViewBag.beginDate = mFilter.Date.Value.ToString("dd.MM.yyyy");

            if (mFilter.DateEnd.HasValue)
                ViewBag.endDate = mFilter.DateEnd.Value.ToString("dd.MM.yyyy");

            return View(ViewName, model);
        }

    }
}