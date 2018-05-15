using PgDbase.entity;
using Portal.Areas.Admin;
using Authentication.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Authentication.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("Auth")]
    public class AuthController : CoreController
    {
        AuthViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);


            model = new AuthViewModel()
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
        
        [Route]
        [Route("Users")]
        public ActionResult Index()
        {
            filter = GetFilter();
            var frontUserFilter = FilterModel.Extend<FrontUserFilter>(filter);
            model.List = _cmsRepository.GetFrontUsers(frontUserFilter);

            return View(model);
        }

        [Route("Users/{id:int}")]
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetFrontUser(id);
            return View(model);
        }
    }
}
