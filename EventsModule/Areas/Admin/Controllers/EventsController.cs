using EventsModule.Areas.Admin.Models;
using PgDbase.entity;
using Portal.Areas.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventsModule.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("Events")]
    public class EventsController : CoreController
    {
        EventViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new EventViewModel
            {
                PageName = PageName,
                DomainName = Domain,
                Account = AccountInfo,
                UserResolution = UserResolutionInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };

            if (AccountInfo != null)
            {
                model.Menu = MenuCmsCore;
                model.MenuModul = MenuModulCore;
            }
            
            //ViewBag.StartUrl = StartUrl;
            ViewBag.Title = "Новости";
        }

        // GET: Admin/Events
        [Route, HttpGet]
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List=_cmsRepository.GetEventsList(filter);
            return View(model);
        }
        [Route, HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, bool enabled, string size, DateTime? datestart, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "searchtext", searchtext);            
            query = AddFilterParam(query, "disabled", (!enabled).ToString().ToLower());
            if (datestart.HasValue)
                query = AddFilterParam(query, "datestart", datestart.Value.ToString("dd.MM.yyyy").ToLower());
            if (dateend.HasValue)
                query = AddFilterParam(query, "dateend", dateend.Value.ToString("dd.MM.yyyy").ToLower());
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);

            return Redirect(StartUrl + query);
        }


        [Route, HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);

            query = AddFilterParam(query, "page", String.Empty);

            return Redirect(StartUrl + "item/" + Guid.NewGuid() + "/" + query);
        }

        [Route("item/{id:guid}"), HttpGet]
        public ActionResult Item(Guid id)
        {
            return View("Item", model);
        }

    }
}