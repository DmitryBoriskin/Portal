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
                PageName = PageName,
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
        public ActionResult Index(Guid subscr)
        {
            filter = GetFilter();
            model.List = _cmsRepository.GetMeterDevices(subscr, filter);
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
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }
    }
}