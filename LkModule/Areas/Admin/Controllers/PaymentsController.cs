using LkModule.Areas.Admin.Models;
using PgDbase.entity;
using Portal.Areas.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("Payments")]
    public class PaymentsController : BeCoreController
    {
        FilterModel filter;
        PaymentsViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new PaymentsViewModel()
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
                Types = _cmsRepository.GetPaymentTypes()
            };
        }

        // GET: Admin/Payments
        [Route]
        public ActionResult Index(Guid? subscr)
        {
            if (subscr == null)
            {
                return Redirect("/admin/subscrs");
            }
            filter = GetFilter();
            model.List = _cmsRepository.GetPayments((Guid)subscr, filter);
            return View(model);
        }

        [Route, HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string size, string page)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);

            return Redirect(StartUrl + query);
        }

        [Route, HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr(Guid subscr)
        {
            return Redirect($"{StartUrl}?subscr={subscr}");
        }
    }
}