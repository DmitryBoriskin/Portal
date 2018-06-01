using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PgDbase;
using PgDbase.entity;
using PgDbase.Repository.cms;
using Portal.Code;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Portal.Areas.Admin
{
    public class FeCoreController : Controller
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


        public FeCoreController()
        {
        }

       
        /// <summary>
        /// Репозиторий для работы с авторизацией
        /// </summary>
        protected AccountRepository _accountRepository { get; private set; }

        /// <summary>
        /// Репозиторий для работы с сущностями
        /// </summary>
        protected CmsRepository _cmsRepository { get; private set; }

        /// <summary>
        /// Домен
        /// </summary>
        public string Domain;
        /// <summary>
        /// Корневая директория
        /// </summary>
        public string SiteDir;

        /// <summary>
        /// Идентификатор сайта
        /// </summary>
        public Guid SiteId;

        /// <summary>
        /// Путь из адресной строки
        /// </summary>
        public string StartUrl;

        /// <summary>
        /// Авторизованный пользователь
        /// </summary>
        public AccountModel AccountInfo;

        /// <summary>
        /// Настройки
        /// </summary>
        public SettingsModel SettingsInfo;

        /// <summary>
        /// Контроллер
        /// </summary>
        public string ControllerName;

        /// <summary>
        /// Действие
        /// </summary>
        public string ActionName;

        /// <summary>
        /// Название страницы
        /// </summary>
        public string PageName;

        /// <summary>
        /// Меню админки из структуры CMS
        /// </summary>
        public CmsMenuItemModel[] MenuCms;

        /// <summary>
        /// Меню модулей
        /// </summary>
        public CmsMenuItemModel[] MenuModule;

        /// <summary>
        /// Права пользователя
        /// </summary>
        public ResolutionModel UserResolutionInfo;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ControllerName = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            ActionName = filterContext.RouteData.Values["Action"].ToString().ToLower();
            PageName = _cmsRepository.GetPageName(ControllerName);

            StartUrl = "/Admin/" + (String)RouteData.Values["controller"] + "/";
            
            #region Данные об авторизованном пользователе
            //Guid _userId = new Guid();
            //try { _userId = new Guid(System.Web.HttpContext.Current.User.Identity.Name); }
            //catch { FormsAuthentication.SignOut(); }
            //UserManager.AddToRole(User.Identity.GetUserId(), SiteId.ToString());

            var userId = User.Identity.GetUserId();
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(userId);

            var _userId = currentUser.UserId;
            //AccountInfo = _accountRepository.getCmsAccount(_userId);

            //Mapping ApplicationUser to AccountModel
            AccountInfo = new AccountModel()
            {
                Name = currentUser.UserInfo.Name,
                Surname = currentUser.UserInfo.Surname,
                Patronymic = currentUser.UserInfo.Patronymic,
                Disabled = currentUser.UserInfo.Disabled,
                Mail = currentUser.Email
            };

            if (AccountInfo != null)
            {
                // Список доменов, доступных пользователю
                AccountInfo.Domains = _accountRepository.GetUserSites(_userId); //_accountRepository.GetSiteLinkUser(_userId);

                // Права доступа пользователя к страницам

                //MenuCms = _cmsRepository.GetCmsMenuItems();
                //MenuModule = _cmsRepository.GetModuleMenu(AccountInfo.Id);
                //UserResolutionInfo = _cmsRepository.GetUserResolutionGroup(AccountInfo.Id, ControllerName);

                //Проверка на права доступа к конкретному сайту
                var siteAuth = User.IsInRole(SiteId.ToString());

                if(!siteAuth)
                    filterContext.Result = new RedirectResult("~/Account/AccessDenied");
            }
            #endregion

        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public FeCoreController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
 
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

            _accountRepository = new AccountRepository("dbConnection",RequestUserInfo.IP, SiteId);

            try
            {
                SiteId = _accountRepository.GetSiteId(domainUrl);
            }
            catch (Exception ex)
            {
                AppLogger.Debug("CoreController: Не получилось определить id сайта", ex);
            }


            _cmsRepository = new CmsRepository("dbConnection", SiteId, RequestUserInfo.IP, userId);

            var core_site = _cmsRepository.GetCoreSites(SiteId);
            SiteDir = core_site.c_serial.ToString();
        }

        /// <summary>
        /// Добавляет параметр
        /// </summary>
        /// <param name="query"></param>
        /// <param name="name"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        protected string AddFilterParam(string query, string name, string val)
        {
            var returnUrl = "";
            //Формируем строку с параметрами фильтра без пейджера и параметра(чтоб убрать дублирование)
            returnUrl = UrlQueryExclude(query, "page");
            returnUrl = UrlQueryExclude(query, name);

            var page = 1;
            if (!string.IsNullOrEmpty(Request.QueryString["page"]))
                int.TryParse(Request.QueryString["page"], out page);

            //Добавляем параметр name
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(val))
                returnUrl = !string.IsNullOrEmpty(returnUrl)
                    ? string.Format("{0}&{1}={2}", returnUrl, name, val)
                    : string.Format("{0}={1}", name, val);

            //Добавляем page
            if (page > 1)
                returnUrl = !string.IsNullOrEmpty(returnUrl)
                    ? string.Format("{0}&page={1}", returnUrl, page)
                    : string.Format("page={0}", page);


            if (!string.IsNullOrEmpty(returnUrl))
            {
                var s = (returnUrl.IndexOf("?") > -1) ? "&" : "?";
                returnUrl = s + returnUrl;
            }

            return returnUrl.ToLower();
        }

        /// <summary>
        /// Формируем новую строку запроса из queryStr="?size=40&searchtext=поиск&page=2" исключая exclude
        /// </summary>
        /// <param name="queryStr"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
        protected string UrlQueryExclude(string queryStr, string exclude)
        {
            if (string.IsNullOrEmpty(queryStr))
                return "";

            var qparams = HttpUtility.ParseQueryString(queryStr);

            var queryParams = new Dictionary<string, string>();
            if (qparams.AllKeys != null && qparams.AllKeys.Count() > 0)
            {
                foreach (var p in qparams.AllKeys)
                {
                    if (p != null)
                        queryParams.Add(p, qparams[p]);
                }
            }

            var urlParams = String.Join("&", queryParams
                            .Where(p => p.Key != exclude)
                            .Select(p => String.Format("{0}={1}", p.Key, p.Value))
                            );

            return urlParams;
        }

        /// <summary>
        /// Возвращает фильтр
        /// </summary>
        /// <param name="defaultPageSize"></param>
        /// <returns></returns>
        protected FilterModel GetFilter(int defaultPageSize = 20)
        {
            var filter = new FilterModel()
            {
                Type = Request.QueryString["type"],
                Category = Request.QueryString["category"],
                Group = Request.QueryString["group"],
                Lang = Request.QueryString["lang"],
                SearchText = Request.QueryString["searchtext"],
                Page = 1,
                Size = defaultPageSize,
                Disabled = null
            };

            var disabled = false;
            if (!String.IsNullOrEmpty(Request.QueryString["disabled"]))
            {
                bool.TryParse(Request.QueryString["disabled"], out disabled);
                filter.Disabled = disabled;
            }

            int page = 1;
            if (!String.IsNullOrEmpty(Request.QueryString["page"]))
            {
                int.TryParse(Request.QueryString["page"], out page);
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

        /// <summary>
        /// Срабатывание на нажатие кнопки с определённым атрибутом
        /// </summary>
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public class MultiButtonAttribute : ActionNameSelectorAttribute
        {
            public string MatchFormKey { get; set; }
            public string MatchFormValue { get; set; }
            public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
            {
                return controllerContext.HttpContext.Request[MatchFormKey] != null &&
                    controllerContext.HttpContext.Request[MatchFormKey] == MatchFormValue;
            }
        }

        private Guid GetCurrentSiteId()
        {
            var _accountRepository = new AccountRepository("dbConnection", RequestUserInfo.IP, Guid.Empty);
            var domainUrl = Request.Url.Host.ToLower().Replace("www.", "");

            var siteId = _accountRepository.GetSiteId(domainUrl);

            if (siteId == Guid.Empty)
                AppLogger.Debug($"CoreController: Не получилось определить Domain для {domainUrl}");

            return siteId;
        }
    }
}