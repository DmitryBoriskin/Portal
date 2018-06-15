using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PgDbase;
using PgDbase.entity;
using PgDbase.Repository;
using PgDbase.Repository.cms;
using Portal.Areas.Admin.Models;
using Portal.Code;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Portal.Areas.Admin.Controllers
{
    [Authorize]
    public class BeCoreController : Controller
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

        /// <summary>
        /// Репозиторий для работы с сущностями
        /// </summary>
        protected CmsRepository _cmsRepository { get; private set; }

        /// <summary>
        /// Домен
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// Корневая директория
        /// </summary>
        public string SiteDir { get; set; }

        /// <summary>
        /// Идентификатор сайта
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Путь из адресной строки
        /// </summary>
        public string StartUrl { get; set; }

        /// <summary>
        /// В админке всегда авторизованный пользователь
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Настройки
        /// </summary>
        public SettingsModel SettingsInfo { get; set; }

        /// <summary>
        /// Контроллер
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Действие
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Название страницы
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// Меню админки из структуры CMS
        /// </summary>
        public CmsMenuItemModel[] MenuCmsCore;

        /// <summary>
        /// Меню модулей
        /// </summary>
        public CmsMenuItemModel[] MenuModulCore;



        public BeCoreController() { }

        //Не используется параметрический вызов
        //public BeCoreController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ControllerName = filterContext.RouteData.Values["Controller"].ToString().ToLower();
            ActionName = filterContext.RouteData.Values["Action"].ToString().ToLower();
            StartUrl = "/Admin/" + (String)RouteData.Values["controller"] + "/";

            // Определяем сайт
            SiteId = GetCurrentSiteId();

            // Данные об авторизованном пользователе
            var _userId = User.Identity.GetUserId();
            var currentUser = UserManager.FindById(_userId);
            var userId = currentUser.UserId;

            //Проверка на права доступа к сайту
            var siteAuth = User.IsInRole(SiteId.ToString());

            //Проверка на права доступа к контроллеру
            var controllerAuth = User.Identity.HasClaim(ControllerName, "view");


            if (!siteAuth || !controllerAuth)
                filterContext.Result = new RedirectResult("~/Account/AccessDenied");


            _cmsRepository = new CmsRepository("dbConnection", SiteId, RequestUserInfo.IP, userId);

            PageName = _cmsRepository.GetPageName(ControllerName);
            if (string.IsNullOrEmpty(PageName))
                PageName = _cmsRepository.GetModulePageName(ControllerName);

            var core_site = _cmsRepository.GetCoreSites(SiteId);
            SiteDir = core_site.c_serial.ToString();

            //Для построения меню в левой части админки
            //Для всех админов один список
            MenuCmsCore = _cmsRepository.GetCmsMenu();
            //Модули для всех - разные
            MenuModulCore = _cmsRepository.GetModulesMenu(userId);

            ViewBag.SiteId = SiteId.ToString();
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

            if (!string.IsNullOrEmpty(Request.QueryString["siteId"]))
            {
                var siteId = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["siteId"], out siteId))
                    filter.SiteId = siteId;
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
            var _repository = new Repository("dbConnection");
            var domainUrl = Request.Url.Host.ToLower().Replace("www.", "");

            var siteId = _repository.GetSiteId(domainUrl);

            if (siteId == Guid.Empty)
                AppLogger.Debug($"CoreController: Не получилось определить Domain для {domainUrl}");

            return siteId;

        }

        /// <summary>
        /// Возвращает кодек
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            var codecs = ImageCodecInfo.GetImageEncoders();
            if (codecs != null && codecs.Count() > 0)
            {
                foreach (var enc in codecs)
                {
                    if (enc.MimeType.ToLower() == mimeType.ToLower())
                        return enc;
                }

            }
            return null;
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (_userManager != null)
        //        {
        //            _userManager.Dispose();
        //            _userManager = null;
        //        }

        //        if (_signInManager != null)
        //        {
        //            _signInManager.Dispose();
        //            _signInManager = null;
        //        }
        //    }

        //    base.Dispose(disposing);
        //}
    }
}