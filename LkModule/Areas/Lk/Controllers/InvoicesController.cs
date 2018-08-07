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
    public class InvoicesController : LayoutController
    {
        FilterModel filter;
        InvoiceFrontModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");


            model = new InvoiceFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };
        }

        //Нплаченные платежи
        public ActionResult Index()
        {
            filter = GetFilter();
            var mFilter = FilterModel.Extend<LkFilter>(filter);

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);
            if (userSubscr != null)
            {
#pragma warning disable CS1061 // 'FrontRepository' does not contain a definition for 'GetInvoices' and no extension method 'GetInvoices' accepting a first argument of type 'FrontRepository' could be found (are you missing a using directive or an assembly reference?)
                model.List = _Repository.GetInvoices(userSubscr.Id, mFilter);
#pragma warning restore CS1061 // 'FrontRepository' does not contain a definition for 'GetInvoices' and no extension method 'GetInvoices' accepting a first argument of type 'FrontRepository' could be found (are you missing a using directive or an assembly reference?)
            }

            if(mFilter.Payed.HasValue)
                ViewBag.payed = mFilter.Payed.Value;

            if (mFilter.Date.HasValue)
                ViewBag.beginDate = mFilter.Date.Value.ToString("dd.MM.yyyy");

            if (mFilter.DateEnd.HasValue)
                ViewBag.endDate = mFilter.DateEnd.Value.ToString("dd.MM.yyyy");

            return View(ViewName, model);
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