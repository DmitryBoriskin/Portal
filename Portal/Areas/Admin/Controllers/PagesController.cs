using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    public class PagesController : BeCoreController
    {
        //public PagesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //    : base(userManager, signInManager) { }

        PageViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new PageViewModel
            {
                SiteId = SiteId,
                PageName = PageName,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                MenuCMS = MenuCmsCore,
                MenuModules = MenuModulCore,
                MenuGroups = _cmsRepository.GetPageGroups()
            };

            #region Метатеги
            ViewBag.Description = "";
            ViewBag.KeyWords = "";
            #endregion
        }

        // GET: Admin/Pages
        public ActionResult Index(string group)
        {
            filter = GetFilter();
            var mfilter = FilterModel.Extend<PageFilterModel>(filter);
            if (!String.IsNullOrEmpty(group))
            {
                mfilter.GroupId = Guid.Parse(group);
                ViewBag.Group = group;
            }

            model.List = _cmsRepository.GetPages(mfilter);

            model.Filter = GetFilterTree();
            return View(model);
        }

        // GET: Admin/Pages/<id>
        [HttpGet]
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetPage(id);
            if (model.Item == null)
            {
                model.Item = new PageModel
                {
                    ParentId = Request.Params["parent"] != null
                                    ? Guid.Parse(Request.Params["parent"]) : Guid.Empty,
                    IsDeleteble = true
                };                
            }
            GetBreadCrumbs(id);
            return View(model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Item(Guid id, PageViewModel backModel, string[] Item_MenuGroups)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };

            string _parent = backModel.Item.ParentId != null ? $"item/{backModel.Item.ParentId.ToString()}" : "";
            string backToListUrl = $"{StartUrl}{_parent}{Request.Url.Query}";

            if (ModelState.IsValid)
            {
                PageModel parentElement = null;

                if ((backModel.Item.ParentId != null))
                {
                    parentElement = _cmsRepository.GetPage((Guid)backModel.Item.ParentId);
                }                

                backModel.Item.Path = (parentElement!=null)?$"{parentElement.Path}{parentElement.Alias}/":"/";
                backModel.Item.Id = id;
                if (Item_MenuGroups != null)
                {
                    backModel.Item.MenuGroups = Item_MenuGroups.Select(s => Guid.Parse(s)).ToArray();
                }
                if (String.IsNullOrWhiteSpace(backModel.Item.Alias))
                {
                    backModel.Item.Alias = backModel.Item.Name;
                }
                backModel.Item.Alias = Transliteration.Translit(backModel.Item.Alias);

                if (_cmsRepository.CheckPageExists(id))
                {
                    _cmsRepository.UpdatePage(backModel.Item);
                    message.Info = "Запись обновлена";
                }
                else
                {
                    _cmsRepository.InsertPage(backModel.Item);
                    message.Info = "Запись добавлена";
                }
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = backToListUrl, Text = "вернуться в список" },
                    new ErrorMessageBtnModel { Url = $"{StartUrl}item/{id}", Text = "ок", Action = "false" }
                };
            }
            else
            {
                message.Info = "Ошибка в заполнении формы. Поля в которых допущены ошибки - помечены цветом";
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = $"{StartUrl}item/{id}", Text = "ок", Action = "false" }
                };
            }

            model.Item = _cmsRepository.GetPage(id);
            GetBreadCrumbs(id);
            model.ErrorInfo = message;
            return View("item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel(Guid id)
        {
            string parent = Request.Form["Item.ParentId"];
            if (parent != null && Guid.Parse(parent) != Guid.Empty)
            {
                parent = $"item/{Request.Form["Item.ParentId"]}";
            }
            else
            {
                parent = null;
            }
            return Redirect($"{StartUrl}{parent}{Request.Url.Query}");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid Id) =>
            Redirect($"{StartUrl}{_cmsRepository.DeletePage(Id)}{Request.Url.Query}");

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

        /// <summary>
        /// Возвращает дерево фильтрации
        /// </summary>
        /// <returns></returns>
        private FilterTreeModel GetFilterTree()
        {
            if (model.MenuGroups != null)
            {
                string link = Request.Url.Query;
                string editGroupUrl = "/admin/services/addfiltertree?section=pages";
                string alias = "group";
                string active = Request.QueryString[alias];
                return new FilterTreeModel()
                {
                    Title = "Группы меню",
                    Icon = "icon-th-list-3",
                    BtnName = "Новая группа меню",
                    Url = editGroupUrl,
                    IsReadOnly = false,
                    //AccountGroup = (model.Account != null) ? model.Account.Group : "",
                    Items = model.MenuGroups.Select(p =>
                        new CatalogList()
                        {
                            Title = p.Title,
                            Alias = p.Id.ToString(),
                            Link = AddFilterParam(link, alias, p.Id.ToString()),
                            Url = $"{editGroupUrl}&id={p.Id}",
                            IsSelected = active == p.Id.ToString()
                        }).ToArray(),
                    Link = "/admin/pages"
                };
            }
            return null;
        }
    }
}