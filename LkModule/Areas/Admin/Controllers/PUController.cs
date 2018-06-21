using LkModule.Areas.Admin.Models;
using Newtonsoft.Json;
using PgDbase.entity;
using Portal.Areas.Admin.Controllers;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace LkModule.Areas.Admin.Controllers
{
    public class PUController : BeCoreController
    {
        FilterModel filter;
        PuViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_cmsRepository.ModuleAllowed(ControllerName))
                Response.Redirect("/Admin/");

            model = new PuViewModel()
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
        public ActionResult Index(Guid? subscr)
        {
            if (subscr == null)
            {
                return Redirect("/admin/subscrs");
            }
            filter = GetFilter();
            model.List = _cmsRepository.GetSubscrDevices((Guid)subscr, filter);
            return View(model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string size, string page, bool enabled)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);
            query = AddFilterParam(query, "disabled", (!enabled).ToString().ToLower());

            return Redirect(StartUrl + query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr(Guid subscr)
        {
            return Redirect($"{StartUrl}?subscr={subscr}");
        }

        [HttpPost]
        public ActionResult GetPuMeters(Guid device)
        {
            var meters = _cmsRepository.GetMeters(device);

            var json = JsonConvert.SerializeObject(meters);
            return Json(json);
        }

        //[HttpPost]
        //public ActionResult GetTariffes(Guid device)
        //{
        //    var tariffes = new TariffModel(); //_cmsRepository.GetTariffes(device);

        //    var json = JsonConvert.SerializeObject(tariffes);
        //    return Json(json);
        //}

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