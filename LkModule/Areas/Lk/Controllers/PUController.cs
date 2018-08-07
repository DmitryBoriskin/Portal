using LkModule.Areas.Lk.Models;
using Newtonsoft.Json;
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
    public class PuController : LayoutController
    {
        FilterModel filter;
        PuFrontModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/Page/error/451");

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
            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            filter = GetFilter();

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);

            if (userSubscr != null)
            {
                model.List = _Repository.GetSubscrDevices(userSubscr.Id, filter);
            }

            return View(ViewName, model);
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

        [HttpPost]
        public ActionResult GetPuMeters(Guid device)
        {
            //ViewName = "/Views/Modules/Pu/Part/CounterReading.cshtml";
            ViewName = "/sites/rushydro/view/module/lk/PU/Part/CounterReading.cshtml";
            var model = _Repository.GetMeters(device);

            //var json = JsonConvert.SerializeObject(meters);
            //return Json(json);

            return PartialView(ViewName, model);
        }

        //[HttpPost]
        //public ActionResult GetTariffes(Guid device)
        //{
        //    var tariffes = _cmsRepository.GetTariffes(device);

        //    var json = JsonConvert.SerializeObject(tariffes);
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