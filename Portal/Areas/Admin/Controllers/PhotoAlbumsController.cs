using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class PhotoAlbumsController : CoreController
    {
        PhotoViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new PhotoViewModel()
            {
                PageName = PageName,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                UserResolution = UserResolutionInfo,
                ActionName = ActionName
            };
            if (AccountInfo != null)
            {
                model.Menu = MenuCmsCore;
                model.MenuModul = MenuModulCore;
            }
        }

        // GET: Admin/PhotoAlbums
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List = _cmsRepository.GetPhotoAlbums(filter);
            return View(model);
        }
    }
}