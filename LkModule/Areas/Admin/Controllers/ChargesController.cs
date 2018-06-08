﻿using LkModule.Areas.Admin.Models;
using PgDbase.entity;
using Portal.Areas.Admin;
using System;
using System.Linq;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("Charges")]
    public class ChargesController : BeCoreController
    {
        FilterModel filter;
        ChargesViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new ChargesViewModel()
            {
                PageName = PageName,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                MenuCMS = MenuCmsCore,
                MenuModules = MenuModulCore,
            };
        }

        // GET: Admin/Charges
        public ActionResult Index(Guid id)
        {
            filter = GetFilter();
            var mFilter = FilterModel.Extend<LkFilter>(filter);
            model.List = _cmsRepository.GetCharges(id, mFilter);

            return View(model);
        }
    }
}