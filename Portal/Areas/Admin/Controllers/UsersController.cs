using PgDbase.Services;
using Portal.Areas.Admin.Models;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class UsersController : CoreController
    {
        UsersViewModel model;
        FilterParams filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new UsersViewModel
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

        // GET: Admin/Users
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List = _cmsRepository.GetUsers(filter);
            return View(model);
        }
    }
}