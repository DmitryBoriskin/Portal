using PgDbase.entity;
using Portal.Areas.Admin;
using TestModule.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace TestModule.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("TestModule")]
    public class TestModuleController : CoreController
    {
        TestModuleViewModel model;
        //FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);


            model = new TestModuleViewModel()
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
