using PgDbase;
using PgDbase.entity;
using PgDbase.Repository.cms;
using Portal.Code;
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
    [Authorize]
    public class CoreController : Controller
    {
        //protected Logger cmsLogger = null;

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
        /// Меню админки из структуры CMS
        /// </summary>
        public CmsMenuModel[] MenuCmsCore;
        /// <summary>
        /// Меню модулей
        /// </summary>
        public CmsMenuItem[] MenuModulCore;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ControllerName = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            ActionName = filterContext.RouteData.Values["Action"].ToString().ToLower();
            
            
            StartUrl = "/Admin/" + (String)RouteData.Values["controller"] + "/";


            #region Данные об авторизованном пользователе
            Guid _userId = new Guid();
            try { _userId = new Guid(System.Web.HttpContext.Current.User.Identity.Name); }
            catch { FormsAuthentication.SignOut(); }
            AccountInfo = _accountRepository.getCmsAccount(_userId);
            if (AccountInfo != null)
            {
                // Список доменов, доступных пользователю
                AccountInfo.Domains = _accountRepository.GetSiteLinkUser(_userId);
                MenuCmsCore = _cmsRepository.GetCmsMenu(AccountInfo.Id);
                MenuModulCore = _cmsRepository.GetModulMenu(AccountInfo.Id);
            }
            #endregion
        }
        
        /// <summary>
        /// Конструктор
        /// </summary>
        public CoreController()
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

            _accountRepository = new AccountRepository("dbConnection",RequestUserInfo.IP, SiteId);

            try
            {
                SiteId = _accountRepository.GetSiteGuid(domainUrl);
            }
            catch (Exception ex)
            {
                AppLogger.Debug("CoreController: Не получилось определить id сайта", ex);
            }


            _cmsRepository = new CmsRepository("dbConnection", userId, RequestUserInfo.IP, SiteId);
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
    }
}