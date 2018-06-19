using LkModule.Areas.Admin.Models;
using PgDbase.entity;
using Portal.Areas.Admin.Controllers;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Controllers
{

    public class AccrualsController : BeCoreController
    {
        FilterModel filter;
        AccrualViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_cmsRepository.ModuleAllowed(ControllerName))
                Response.Redirect("/Admin/");

            model = new AccrualViewModel()
            {
                SiteId = SiteId,
                PageName = "Выставленные счета",
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                MenuCMS = MenuCmsCore,
                MenuModules = MenuModulCore,
            };
        }

        public ActionResult Index(Guid? subscr)
        {
            if (subscr == null)
            {
                return Redirect("/admin/subscrs");
            }
            filter = GetFilter();
            var mFilter = FilterModel.Extend<LkFilter>(filter);
            bool payed = false;
            if (!String.IsNullOrEmpty(Request.QueryString["payed"]))
            {
                bool.TryParse(Request.QueryString["payed"], out payed);
                mFilter.Payed = payed;
            }
            model.List = _cmsRepository.GetAccruals((Guid)subscr, mFilter);

            return View(model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string size, string page, bool payed)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);
            query = AddFilterParam(query, "payed", payed.ToString().ToLower());

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


        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetAccrual(id);
            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id)
        {
            var subscr = _cmsRepository.GetSubscrByAccrual(id);
            string url = "/admin/charges/?subscr=";
            if (subscr != null && subscr != Guid.Empty)
            {
                url += subscr;
            }
            return Redirect(url);
        }
    }
}