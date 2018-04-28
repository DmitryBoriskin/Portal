using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class MenuController : CoreController
    {
        MenuViewModel model;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new MenuViewModel
            {
                DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };
            if (AccountInfo != null)
            {
                model.Menu = _cmsRepository.GetCmsMenu(AccountInfo.Id);
            }
        }



        // GET: Admin/Menu
        public ActionResult Index()
        {
            model.MenuList = _cmsRepository.GetCmsMenu();
            return View();
        }
    }
}