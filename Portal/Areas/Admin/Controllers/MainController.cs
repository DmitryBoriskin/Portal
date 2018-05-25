using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    [Authorize]
    public class MainController : BeCoreController
    {

       // GET: Admin/Main
        public ActionResult Index()
        {
            MainViewModel model = new MainViewModel()
            {
                PageName = PageName,
                DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
            };
            if (AccountInfo != null)
            {
                model.Menu = MenuCmsCore;
                model.MenuModul = MenuModulCore;
            }                
             //model.AccountLog = _cmsRepository.getCmsUserLog(AccountInfo.Id);

            return View(model);
        }

        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}