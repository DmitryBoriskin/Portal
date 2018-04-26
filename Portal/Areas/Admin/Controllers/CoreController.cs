using PgDbase;
using PgDbase.entity;
using PgDbase.entity.cms;
using PgDbase.Repository.cms;
using PgDbase.Services;
using Portal.Code;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Portal.Areas.Admin.Controllers
{
    [Authorize]
    public class CoreController : Controller
    {
        //protected Logger cmsLogger = null;
        /// <summary>
        /// Контекст доступа к базе данных
        /// </summary>
        protected AccountRepository _accountRepository { get; private set; }
        protected CmsRepository _cmsRepository { get; private set; }

        public string Domain;
        public Guid SiteId;
        public string StartUrl;
        public AccountModel AccountInfo;
        public SettingsModel SettingsInfo;
        public string ControllerName;
        public string ActionName;



        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ControllerName = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            ActionName = filterContext.RouteData.Values["Action"].ToString().ToLower();


            try
            {
                SiteId = _cmsRepository.GetSiteGuid(Request.Url.Host.ToLower().Replace("www.", ""));                
            }
            catch (Exception ex)
            {
                if (Request.Url.Host.ToLower().Replace("www.", "") != ConfigurationManager.AppSettings["BaseURL"])
                    filterContext.Result = Redirect("/Error/");
                else Domain = String.Empty;

                AppLogger.Debug("CoreController: Не получилось определить Domain", ex);
            }
            StartUrl = "/Admin/" + (String)RouteData.Values["controller"] + "/";


            #region Данные об авторизованном пользователе
            Guid _userId = new Guid();
            try { _userId = new Guid(System.Web.HttpContext.Current.User.Identity.Name); }
            catch { FormsAuthentication.SignOut(); }
            AccountInfo = _accountRepository.getCmsAccount(_userId);

            // Список доменов, доступных пользователю
            AccountInfo.Domains = _accountRepository.GetSiteLinkUser(_userId);
            #endregion
        }



        public CoreController()
        {
            _accountRepository = new AccountRepository("dbConnection");

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
            _cmsRepository = new CmsRepository("dbConnection", userId, RequestUserInfo.IP, SiteId);
        }

        /// <summary>
        /// Возвращает фильтр
        /// </summary>
        /// <param name="defaultPageSize"></param>
        /// <returns></returns>
        public FilterParams GetFilter(int defaultPageSize = 20)
        {
            FilterParams filter = new FilterParams()
            {
                Domain = Domain,
                Type = Request.QueryString["type"],
                Category = Request.QueryString["category"],
                Group = Request.QueryString["group"],
                Lang = Request.QueryString["lang"],
                SearchText = Request.QueryString["searchtext"],
                Page = 1,
                Size = defaultPageSize,
                Disabled = null
            };

            if (!String.IsNullOrEmpty(Request.QueryString["disabled"]))
            {
                bool.TryParse(Request.QueryString["disabled"], out bool disabled);
                filter.Disabled = disabled;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["page"]))
            {
                int.TryParse(Request.QueryString["page"], out int page);
                if (page > 1)
                    filter.Page = page;
            }
            if (!String.IsNullOrEmpty(Request.QueryString["size"]))
            {
                int size = defaultPageSize;
                int.TryParse(Request.QueryString["size"], out size);
                if (size > 0)
                    filter.Size = size;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["datestart"]))
            {
                DateTime datestart = DateTime.MinValue;
                var res = DateTime.TryParse(Request.QueryString["datestart"], out datestart);

                if (datestart != DateTime.MinValue)
                    filter.Date = datestart;
            }

            if (!String.IsNullOrEmpty(Request.QueryString["dateend"]))
            {
                DateTime dateend = DateTime.MinValue;
                DateTime.TryParse(Request.QueryString["dateend"], out dateend);

                if (dateend != DateTime.MinValue)
                    filter.DateEnd = dateend;
            }

            return filter;
        }
    }
}