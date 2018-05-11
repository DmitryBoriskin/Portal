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
            model.Filter = GetFilterTree();
            return View(model);
        }
        

        //GET: Admin/news/item/{GUID}
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetNewsItem(id);
            return View("Item", model);
        }

        #region категории
        private FilterTreeModel GetFilterTree()
        {
            if (model.Category != null)
            {
                string link = Request.Url.Query;
                string editGroupUrl = "/admin/news/Category/";
                string alias = "category";
                string active = Request.QueryString[alias];
                return new FilterTreeModel()
                {
                    Title = "Категории",
                    Icon = "icon-rh-list-3",
                    IsReadOnly = false,
                    Items = model.Category.Select(p =>
                          new CatalogList()
                          {
                              Title = p.Name,
                              Alias = p.Alias,
                              Link = AddFilterParam(link, alias, p.Alias.ToString()),
                              Url = $"{editGroupUrl}{p.Id}",
                              IsSelected = active == p.Id.ToString()
                          }).ToArray(),
                    Link = "/admin/news"
                };
            }
            return null;
        }
        public ActionResult Category(Guid? id)
        {
            if (id != null)
            {
                model.CategoryItem = _cmsRepository.GetNewsCategoryItem((Guid)id);
            }
            return View(model.CategoryItem);
        }
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-group-btn")]
        public ActionResult Category(NewsCategoryModel model)
        {
            if (_cmsRepository.ExistNewsCategory(model.Id))
            {
                //обновляем
                _cmsRepository.UpdateNewsCategory(model);

            }
            else
            {
                //создаем
                _cmsRepository.InsertNewsCaetegory(model);
            }
            
            return View(model);
        }
        #endregion
    }
}