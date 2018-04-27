using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin
{
    public class MainController : CoreController
    {
        // GET: Admin/Main
        public ActionResult Index()
        {
            MainViewModel model = new MainViewModel()
            {
                DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
            };
            if (AccountInfo != null)
            {
                model.Menu = _cmsRepository.GetCmsMenu(AccountInfo.Id);
                //model.AccountLog = _cmsRepository.getCmsUserLog(AccountInfo.Id);
            }

            return View(model);
        }
    }
}