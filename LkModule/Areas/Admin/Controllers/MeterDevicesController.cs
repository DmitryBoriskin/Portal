using LkModule.Areas.Admin.Models;
using PgDbase.entity;
using Portal.Areas.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace LkModule.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("MeterDevices")]
    public class MeterDevicesController : BeCoreController
    {
        FilterModel filter;
        MeterDeviceViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new MeterDeviceViewModel()
            {
                SiteId = SiteId,
                PageName = "Приборы учёта",
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                MenuCMS = MenuCmsCore,
                MenuModules = MenuModulCore,
            };
        }

        // GET: Admin/MeterDevices
        [Route]
        public ActionResult Index(Guid? subscr)
        {
            if (subscr == null)
            {
                return Redirect("/admin/subscrs");
            }
            filter = GetFilter();
            model.List = _cmsRepository.GetMeterDevices((Guid)subscr, filter);
            return View(model);
        }

        [Route, HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string size, string page, bool enabled)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);
            query = AddFilterParam(query, "disabled", (!enabled).ToString().ToLower());

            return Redirect(StartUrl + query);
        }

        [Route, HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr(Guid subscr)
        {
            return Redirect($"{StartUrl}?subscr={subscr}");
        }

        [Route("GetInfo"), HttpPost]
        public ActionResult GetInfo(Guid device)
        {
            var meters = _cmsRepository.GetMeters(device);

            var json = new JavaScriptSerializer().Serialize(meters);
            return Json(json);
        }

        [Route("GetTariffes"), HttpPost]
        public ActionResult GetTariffes(Guid device)
        {
            var tariffes = _cmsRepository.GetTariffes(device);

            var json = new JavaScriptSerializer().Serialize(tariffes);
            return Json(json);
        }

        [Route, HttpPost]
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