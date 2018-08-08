using LkModule.Areas.Admin.Models;
using PgDbase.entity;
using Portal.Areas.Admin.Controllers;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Controllers
{

    public class InvoicesController : BeCoreController
    {
        FilterModel filter;
        InvoiceViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_cmsRepository.ModuleAllowed(ControllerName))
                Response.Redirect("/Admin/");

            model = new InvoiceViewModel()
            {
                SiteId = SiteId,
                PageName = "Счета-фактуры",
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
           
            if (!String.IsNullOrEmpty(Request.QueryString["payed"]))
            {
                var res = bool.TryParse(Request.QueryString["payed"], out bool payed);
                if (res)
                    mFilter.Payed = payed;
            }
            model.List = _cmsRepository.GetInvoices((Guid)subscr, mFilter);
           

            if (mFilter.Date.HasValue)
                ViewBag.beginDate = mFilter.Date.Value.ToString("dd.MM.yyyy");

            if (mFilter.DateEnd.HasValue)
                ViewBag.endDate = mFilter.DateEnd.Value.ToString("dd.MM.yyyy");

            if (mFilter.Payed.HasValue)
                ViewBag.payed = mFilter.Payed.Value.ToString();

            return View(model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string size, string page, bool? payed, string datestart, string dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);
            query = AddFilterParam(query, "payed", payed.ToString().ToLower());

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


        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetInvoice(id);
            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id)
        {
            var subscr = _cmsRepository.GetSubscrByInvoice(id);
            string url = "/admin/charges/?subscr=";
            if (subscr != null && subscr != Guid.Empty)
            {
                url += subscr;
            }
            return Redirect(url);
        }
    }
}