﻿using LkModule.Areas.Admin.Models;
using PgDbase.entity;
using Portal.Areas.Admin.Controllers;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Controllers
{
    //[RouteArea("Admin")]
    //[RoutePrefix("Charges")]
    public class ChargesController : BeCoreController
    {
        FilterModel filter;
        ChargesViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_cmsRepository.ModuleAllowed(ControllerName))
                Response.Redirect("/Admin/");

            model = new ChargesViewModel()
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

        //[Route]
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
            model.List = _cmsRepository.GetCharges((Guid)subscr, mFilter);

            return View(model);
        }

        //[Route]
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

        //[Route]
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr(Guid subscr)
        {
            return Redirect($"{StartUrl}?subscr={subscr}");
        }
    }
}