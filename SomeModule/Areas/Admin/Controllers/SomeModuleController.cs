using PgDbase.entity;
using Portal.Areas.Admin;
using SomeModule.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SomeModule.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("SomeModule")]
    public class SomeModuleController : CoreController
    {
        SomeModuleViewModel model;
        //FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);


            model = new SomeModuleViewModel()
            {
                PageName = PageName,
                DomainName = Domain,
                //DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };

            if (AccountInfo != null)
            {
                model.Menu = MenuCmsCore;
                model.MenuModul = MenuModulCore;
            }
        }

        // GET: Admin/SomeModule
        [Route]
        public ActionResult Index()
        {
            return View(model);
        }
    }
}
