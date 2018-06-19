﻿using LkModule.Areas.Admin.Models;
using LkModule.Areas.Lk.Models;
using PgDbase.entity;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LkModule.Areas.Lk.Controllers
{
    public class PuController : LayoutController
    {
        FilterModel filter;
        PuFrontModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            //if (!_Repository.ModuleAllowed(ControllerName))
            //    Response.Redirect("/Page/ModuleDenied");

            model = new PuFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };
        }

        // GET: Admin/MeterDevices
        public ActionResult Index()
        {
            filter = GetFilter();

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);

            if (userSubscr != null)
            {
                model.List = _Repository.GetSubscrDevices(userSubscr.Id, filter);
            }

            return View(model);
        }

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        //public ActionResult Search(string size, string page, bool enabled)
        //{
        //    string query = HttpUtility.UrlDecode(Request.Url.Query);
        //    query = AddFilterParam(query, "page", String.Empty);
        //    query = AddFilterParam(query, "size", size);
        //    query = AddFilterParam(query, "disabled", (!enabled).ToString().ToLower());

        //    return Redirect(StartUrl + query);
        //}

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        //public ActionResult ClearFiltr(Guid subscr)
        //{
        //    return Redirect($"{StartUrl}?subscr={subscr}");
        //}

        //[HttpPost]
        //public ActionResult GetInfo(Guid device)
        //{
        //    var meters = _cmsRepository.GetMeters(device);

        //    var json = new JavaScriptSerializer().Serialize(meters);
        //    return Json(json);
        //}

        //[HttpPost]
        //public ActionResult GetTariffes(Guid device)
        //{
        //    var tariffes = _cmsRepository.GetTariffes(device);

        //    var json = new JavaScriptSerializer().Serialize(tariffes);
        //    return Json(json);
        //}

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "back-btn")]
        //public ActionResult Back()
        //{
        //    string par = Request.UrlReferrer.Query;
        //    string subscr = par.Replace("?subscr=", "");
        //    string url = $"/admin/subscrs/item/{subscr}";
        //    return Redirect(url);
        //}
    }
}