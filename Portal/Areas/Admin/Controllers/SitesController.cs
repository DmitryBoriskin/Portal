using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System.Web.Mvc;

namespace Portal.Areas.Admin
{
    public class SitesController : CoreController
    {
        SitesViewModel model;
        FilterModel filter;

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
            PgDbase.entity.FilterModel filter = GetFilter();
            model.List = _cmsRepository.GetSitesList(filter);

            return View(model);
        }
    }
}