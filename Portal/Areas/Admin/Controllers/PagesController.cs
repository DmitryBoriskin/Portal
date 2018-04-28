using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class PagesController : CoreController
    {
        PageViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new PageViewModel
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

            #region Метатеги
            //ViewBag.Title = UserResolutionInfo.Title;
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Admin/Pages
        public ActionResult Index()
        {
            var mfilter = FilterModel.Extend<ModuleFilter>(filter);
            model.List = _cmsRepository.GetPages(model.Filter);
            return View(model);
        }

        // GET: Admin/Pages/<id>
        [HttpGet]
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetPage(id);
            GetBreadCrumbs(id);
            return View(model);
        }

        /// <summary>
        /// Возвращает хлебные крошки
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private void GetBreadCrumbs(Guid id)
        {
            model.BreadCrumbs = new BreadCrumb
            {
                Title = "Главная",
                DefaultUrl = StartUrl,
                Items = _cmsRepository.GetBreadCrumbs(id)
            };
        }
    }
}