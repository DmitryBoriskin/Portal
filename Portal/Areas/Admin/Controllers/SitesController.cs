using PgDbase.Services;
using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class SitesController : CoreController
    {
        SitesViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new SitesViewModel
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


        // GET: Admin/Sites
        public ActionResult Index()
        {
            FilterParams filter = GetFilter();
            model.List = _cmsRepository.GetSitesList(filter);

            return View(model);
        }
    }
}