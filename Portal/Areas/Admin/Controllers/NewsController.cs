using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
            //иерархия категорий            
            model.Filter = GetFilterTree();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchtext"></param>
        /// <param name="disabled"></param>
        /// <param name="size"></param>
        /// <param name="date"></param>
        /// <param name="dateend"></param>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, string group, bool enabled, string size, DateTime? datestart, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "searchtext", searchtext);
            query = AddFilterParam(query, "category", searchtext);
            query = AddFilterParam(query, "disabled", (!enabled).ToString().ToLower());
            if (datestart.HasValue)
                query = AddFilterParam(query, "datestart", datestart.Value.ToString("dd.MM.yyyy").ToLower());
            if (dateend.HasValue)
                query = AddFilterParam(query, "dateend", dateend.Value.ToString("dd.MM.yyyy").ToLower());
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);

            return Redirect(StartUrl + query);
        }

        /// <summary>
        /// Очищаем фильтр
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }

        //GET: Admin/news/item/{GUID}
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetNewsItem(id);
            var Date = DateTime.Today;            
            if (model.Item != null)
            {
                ViewBag.Photo = model.Item.Photo;
                Date = model.Item.Date;
            }
            ViewBag.DataPath = Settings.UserFiles + SiteDir + Settings.MaterialsDir + Date.ToString("yyyy_mm") + "/" + Date.ToString("dd") + "/" + id + "/";
            return View("Item", model);
        }


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);

            return Redirect(StartUrl + "item/" + Guid.NewGuid() + "/" + query);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, NewsViewModel backModel, HttpPostedFileBase upload)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            backModel.Item.Guid = id;
            if (ModelState.IsValid)
            {
                #region Изображение
                if (upload != null)
                {
                    #region добавление изображения
                    string Path = Settings.UserFiles + SiteDir + Settings.MaterialsDir + backModel.Item.Date.ToString("yyyy_mm") + "/" + backModel.Item.Date.ToString("dd") + "/" + id + "/";

                    #region оригинал
                    string PathOriginal = Path + "original/";
                    if (!Directory.Exists(PathOriginal)) { Directory.CreateDirectory(Server.MapPath(PathOriginal)); }
                    upload.SaveAs(Server.MapPath(Path + "original/" + upload.FileName));
                    #endregion

                    if (upload != null && upload.ContentLength > 0)
                        try
                        {
                            backModel.Item.Photo = Files.SaveImageResize(upload, Path, 540, 360);
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "Произошла ошибка: " + ex.Message.ToString();
                        }
                    else
                    {
                        ViewBag.Message = "Вы не выбрали файл.";
                    }
                    #endregion
                }
                #endregion
                //категория
                if (Request["Item.Services"] != null)
                {
                    backModel.Item.CategoryId = Request["Item.Services"].Split(',').Select(s => Guid.Parse(s)).ToArray();
                }
                if (_cmsRepository.CheckNews(id))
                {
                    _cmsRepository.UpdateNews(backModel.Item);
                    message.Info = "Запись обновлена";
                }
                else
                {
                    _cmsRepository.InsertNews(backModel.Item);
                    message.Info = "Запись добавлена";
                }
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = StartUrl + Request.Url.Query, Text = "вернуться в список" },
                    new ErrorMessageBtnModel { Url = StartUrl + "item/"+id, Text = "ок", Action = "false" }
                };
            }
            else
            {
                message.Info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом";
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = "#", Text = "ок", Action = "false" }
                };
            }
            var DataDir = DateTime.Today;
            model.Item = _cmsRepository.GetNewsItem(id);
            if (model.Item != null)
            {
                ViewBag.Photo = model.Item.Photo;
                DataDir = model.Item.Date;
            }
            ViewBag.DataPath = Settings.UserFiles + SiteDir + Settings.MaterialsDir + DataDir.ToString("yyyy_mm") + "/" + DataDir.ToString("dd") + "/" + id + "/";

            model.ErrorInfo = message;
            return View("item", model);
        }


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            _cmsRepository.DeleteNews(id);
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация",
                Info = "Запись удалена",
                Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = StartUrl + Request.Url.Query, Text = "ок", Action = "false" }
                }
            };
            model.ErrorInfo = message;
            return RedirectToAction("index");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
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
            if (ModelState.IsValid)
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
                ViewBag.SuccesAlert = "Запись сохранена.";
            }
            else
            {
                ViewBag.DankerAler = "Произошла ошибка.";
            }
            return View(model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-group-btn")]
        public ActionResult CategoryDelete(Guid id)
        {
            if (_cmsRepository.DeleteNewsCategory(id))
            {
                ViewBag.SuccesAlert = "Категория успешно удалена.";
            }
            else
            {
                ViewBag.DankerAler = "Произошла ошибка.";
            }
            return View();            
        }
        #endregion
    }
}