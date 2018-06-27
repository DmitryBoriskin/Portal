using LkModule.Areas.Lk.Models;
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
    public class AccrualsController : LayoutController
    {
        FilterModel filter;
        AccrualFrontModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            //if (!_Repository.ModuleAllowed(ControllerName))
            //    Response.Redirect("/Page/ModuleDenied");

            model = new AccrualFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };
        }

        //Неоплаченные платежи
        public ActionResult Index()
        {
            //Шаблон
            var view = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(view))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            filter = GetFilter();
            var mFilter = FilterModel.Extend<LkFilter>(filter);
            mFilter.Payed = false;

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);
            if (userSubscr != null)
            {
                model.List = _Repository.GetAccruals(userSubscr.Id, mFilter);
            }

            if (mFilter.Date.HasValue)
                ViewBag.beginDate = mFilter.Date.Value.ToString("dd.MM.yyyy");

            if (mFilter.DateEnd.HasValue)
                ViewBag.endDate = mFilter.DateEnd.Value.ToString("dd.MM.yyyy");

            return View(view, model);
        }

        //Оплаченные платежи
        public ActionResult Payed()
        {
            //Шаблон
            var view = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(view))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            filter = GetFilter();
            var mFilter = FilterModel.Extend<LkFilter>(filter);
            mFilter.Payed = true;

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);
            if (userSubscr != null)
            {
                model.List = _Repository.GetAccruals(userSubscr.Id, mFilter);
            }

            if (mFilter.Date.HasValue)
                ViewBag.beginDate = mFilter.Date.Value.ToString("dd.MM.yyyy");

            if (mFilter.DateEnd.HasValue)
                ViewBag.endDate = mFilter.DateEnd.Value.ToString("dd.MM.yyyy");

            return View(view, model);
        }

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        //public ActionResult Search(string size, string page, string status, string type)
        //{
        //    string query = HttpUtility.UrlDecode(Request.Url.Query);
        //    query = AddFilterParam(query, "page", String.Empty);
        //    query = AddFilterParam(query, "size", size);
        //    query = AddFilterParam(query, "status", status);
        //    query = AddFilterParam(query, "type", type);

        //    return Redirect(StartUrl + query);
        //}

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        //public ActionResult ClearFiltr(Guid subscr)
        //{
        //    return Redirect($"{StartUrl}?subscr={subscr}");
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