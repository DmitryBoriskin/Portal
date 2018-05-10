using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class NewsController : CoreController
    {
        NewsViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new NewsViewModel
            {
                PageName = PageName,
                DomainName = Domain,
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

            model.Category = _cmsRepository.GetNewsCategory();
            //ViewBag.StartUrl = StartUrl;
            ViewBag.Title = "Новости";
        }

        // GET: Admin/News
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List = _cmsRepository.GetNewsList(filter);
            return View(model);
        }
        public ActionResult Category(Guid? id)
        {
            return View();
        }
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-group-btn")]        
        public ActionResult Category(NewsCategoryModel model)
        {
            return View(model);
        }

        //GET: Admin/news/item/{GUID}
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetNewsItem(id);
            return View("Item", model);
        }
    }
}