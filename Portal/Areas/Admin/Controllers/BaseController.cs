using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PgDbase;
using PgDbase.Repository;
using PgDbase.Repository.cms;
using Portal.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Portal.Areas.Admin.Controllers
{
    public class BaseController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public Guid SiteId;

        protected CmsRepository _cmsRepository { get; private set; }

        public BaseController()
        {
            //SiteId = GetCurrentSiteId();
            //_cmsRepository = new CmsRepository("dbConnection", SiteId, RequestUserInfo.IP, Guid.Empty);
        }

        public BaseController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;

            // Данные об авторизованном пользователе
            //var _userId = User.Identity.GetUserId();
            //var currentUser = UserManager.FindById(_userId);
            //var userId = currentUser.UserId;

            //_cmsRepository = new CmsRepository("dbConnection", SiteId, RequestUserInfo.IP, userId);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Определяем сайт
            SiteId = GetCurrentSiteId();

            //PageName = _Repository.GetPageName(ControllerName);
            //StartUrl = "/Admin/" + (String)RouteData.Values["controller"] + "/";

            //// Данные об авторизованном пользователе
            //var userId = User.Identity.GetUserId();
            ////var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //var currentUser = UserManager.FindById(userId);
            //var _userId = currentUser.UserId;

            ////Проверка на права доступа к конкретному сайту
            //var siteAuth = User.IsInRole(SiteId.ToString());

            //if(!siteAuth)
            //    filterContext.Result = new RedirectResult("~/Account/AccessDenied");

        }

        public BaseController()
        {
            _cmsRepository = new CmsRepository("dbConnection", Guid.Empty, RequestUserInfo.IP, SiteId);
        }

        public BaseController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;

            // Данные об авторизованном пользователе
            var _userId = User.Identity.GetUserId();
            var currentUser = UserManager.FindById(_userId);
            var userId = currentUser.UserId;
            _cmsRepository = new CmsRepository("dbConnection", SiteId, RequestUserInfo.IP, userId);
        }

        private Guid GetCurrentSiteId()
        {
            var _repository = new Repository("dbConnection");
            var domainUrl = Request.Url.Host.ToLower().Replace("www.", "");

            var siteId = _repository.GetSiteId(domainUrl);

            if (siteId == Guid.Empty)
                AppLogger.Debug($"BaseController: Не получилось определить Domain для {domainUrl}");

            return siteId;

        }
    }
}