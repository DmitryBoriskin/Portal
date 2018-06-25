using LkModule.Areas.Admin.Models;
using PgDbase.entity;
using Portal.Areas.Admin;
using Portal.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Controllers
{
    public class PaymentsController : BeCoreController
    {
        FilterModel filter;
        PaymentViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_cmsRepository.ModuleAllowed(ControllerName))
                Response.Redirect("/Admin/");

            model = new PaymentViewModel()
            {
                SiteId = SiteId,
                PageName = "Поступившие платежи",
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                MenuCMS = MenuCmsCore,
                MenuModules = MenuModulCore,
                Statuses = _cmsRepository.GetPaymentStatuses(),
                //Types = _cmsRepository.GetPaymentTypes()
            };
        }

        // GET: Admin/Payments
        public ActionResult Index(Guid? subscr)
        {
            if (subscr == null)
            {
                return Redirect("/admin/subscrs");
            }
            filter = GetFilter();
            var mFilter = FilterModel.Extend<LkFilter>(filter);
            mFilter.Status = ViewBag.Status = Request.Params["status"];
            mFilter.Type = ViewBag.Type = Request.Params["type"];


            model.List = _cmsRepository.GetPayments((Guid)subscr, mFilter);


            if (mFilter.Date.HasValue)
                ViewBag.beginDate = mFilter.Date.Value.ToString("dd.MM.yyyy");

            if (mFilter.DateEnd.HasValue)
                ViewBag.endDate = mFilter.DateEnd.Value.ToString("dd.MM.yyyy");


            return View(model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string size, string page, string status, string type, string datestart, string dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);
            query = AddFilterParam(query, "status", status);
            query = AddFilterParam(query, "type", type);

            query = AddFilterParam(query, "datestart", datestart);
            query = AddFilterParam(query, "dateend", dateend);

            return Redirect(StartUrl + query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr(Guid subscr)
        {
            return Redirect($"{StartUrl}?subscr={subscr}");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "back-btn")]
        public ActionResult Back()
        {
            string par = Request.UrlReferrer.Query;
            string subscr = par.Replace("?subscr=", "");
            string url = $"/admin/subscrs/item/{subscr}";
            return Redirect(url);
        }
    }
}