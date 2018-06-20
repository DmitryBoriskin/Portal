using Portal.Areas.Admin.Controllers;
using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MessagesModule.Areas.Admin.Models;

namespace MessagesModule.Areas.Admin.Controllers
{
    public class MessagesController : BeCoreController
    {
        MessagesViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_cmsRepository.ModuleAllowed(ControllerName))
                Response.Redirect("/Admin/");

            model = new MessagesViewModel()
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
        // GET: Admin/Messages
        public ActionResult Index()
        {
            return View();
        }
    }
}