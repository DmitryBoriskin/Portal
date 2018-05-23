using PgDbase;
using PgDbase.Repository.cms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Portal.Areas.Admin.Controllers
{
    public class ConnectionController : Controller
    {
        public Guid SiteId;
        protected CmsRepository _cmsRepository { get; private set; }
        protected AccountRepository _accountRepository { get; private set; }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            Guid _userId = new Guid();
            try { _userId = new Guid(System.Web.HttpContext.Current.User.Identity.Name); }
            catch { FormsAuthentication.SignOut(); }
        }

        public ConnectionController()
        {
            Guid userId = Guid.Empty;
            var domainUrl = "";
            if (System.Web.HttpContext.Current != null)
            {
                var context = System.Web.HttpContext.Current;
                if (context.Request != null && context.Request.Url != null && !string.IsNullOrEmpty(context.Request.Url.Host))
                    domainUrl = context.Request.Url.Host.ToLower().Replace("www.", "");
                if (context.User != null && context.User.Identity != null && !string.IsNullOrEmpty(context.User.Identity.Name))
                {
                    try
                    {
                        userId = Guid.Parse(System.Web.HttpContext.Current.User.Identity.Name);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Не удалось определить идентификатор пользователя" + ex);
                    }
                }
            }
            _accountRepository = new AccountRepository("dbConnection", RequestUserInfo.IP, SiteId);
            try
            {
                SiteId = _accountRepository.GetSiteGuid(domainUrl);
            }
            catch (Exception ex)
            {
                //AppLogger.Debug("CoreController: Не получилось определить id сайта", ex);
            }
            _cmsRepository = new CmsRepository("dbConnection", userId, RequestUserInfo.IP, SiteId);
        }

        // GET: Admin/Connection
        public ActionResult Index()
        {
            return View();
        }
    }
}