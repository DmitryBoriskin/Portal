using Microsoft.AspNet.Identity;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Portal.Controllers.AccountManageController;

namespace Portal.Controllers
{
    public class SettingsController : LayoutController
    {
        private SettingsFrontModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new SettingsFrontModel()
            {
                LayoutInfo = _layoutData,
                User = CurrentUser,
                Breadcrumbs = _breadcrumb
            };
        }
        // GET: Settings
        public ActionResult Index()
        {
            model.UserModel = _Repository.GetUser(CurrentUser.UserId);
            return View(model);
        }
      
        
    }
}