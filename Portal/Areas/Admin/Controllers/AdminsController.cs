using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    [Authorize(Roles = "Developer,PortalAdmin")] 
    public class AdminsController : BeCoreController
    {

        //Работать с администраторами в рамках портала могут только Developer и PortalAdmin
       
        UsersViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new UsersViewModel()
            {
                PageName = PageName,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                
            };
            if (AccountInfo != null)
            {
                model.Menu = MenuCmsCore;
                model.MenuModules = MenuModulCore;
            }

            //Исключаем из выборки вышестоящие роли, например PortalAdmin не должен видеть Developer
            string[] excludeRoles = null;

            if (User.IsInRole("PortalAdmin"))
                excludeRoles = new string[] { "Developer" };
            //else developer
            model.Roles = _cmsRepository.GetRoles(excludeRoles);

        }

        // GET: Admins/
        public ActionResult Index()
        {
            filter = GetFilter();
            model.Filter = GetFilterTree();

            var userFilter = FilterModel.Extend<UserFilter>(filter);

            //Исключаем из выборки пользователей вышестоящих ролей, например PortalAdmin не должен видеть Developer
            if (User.IsInRole("PortalAdmin"))
                userFilter.ExcludeRoles = new string[] { "Developer" };
            //else developer

            model.List = _cmsRepository.GetPortalAdmins(userFilter);

            return View(model);
        }

        // GET: Admin/item/<id>
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetUser(id);
            model.Item.Roles = _cmsRepository.GetUserRoles(id);
            model.Item.Sites = _cmsRepository.GetUserSites(id);
            model.Sites = _cmsRepository.GetSites();
            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);

            return Redirect($"{StartUrl}item/{Guid.NewGuid()}/{query}");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, UsersViewModel backModel)
        {

            //Здесь фактически привязка пользователя к роли и к сайтам
            //Выборка из всех имеющихся пользователей портала
            //Пользователь должен быть зарегистрирован на одном из сайтов

            //ErrorMessage message = new ErrorMessage
            //{
            //    Title = "Информация"
            //};
            //if (ModelState.IsValid)
            //{
            //    backModel.Item.Id = id;
            //    if (_cmsRepository.CheckUserExists(id))
            //    {
            //        _cmsRepository.UpdateUser(backModel.Item);
            //        message.Info = "Запись обновлена";
            //    }
            //    else if (_cmsRepository.CheckUserExists(backModel.Item.Email))
            //    {
            //        message.Info = "Пользователь с таким Email адресом уже существует";
            //    }
            //    else
            //    {
            //        _cmsRepository.InsertUser(backModel.Item);

            //        message.Info = "Запись добавлена";
            //    }
            //    message.Buttons = new ErrorMessageBtnModel[]
            //    {
            //        new ErrorMessageBtnModel { Url = StartUrl + Request.Url.Query, Text = "вернуться в список" },
            //        new ErrorMessageBtnModel { Url = $"{StartUrl}/item/{id}", Text = "ок", Action = "false" }
            //    };
            //}
            //else
            //{
            //    message.Info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом";
            //    message.Buttons = new ErrorMessageBtnModel[]
            //    {
            //        new ErrorMessageBtnModel { Url = $"{StartUrl}/item/{id}", Text = "ок", Action = "false" }
            //    };
            //}

            model.Item = _cmsRepository.GetUser(id);
            //model.ErrorInfo = message;
            return View("item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid Id)
        {

            //Удаление у пользователя роли и привязанных сайтов (опционально)
            //_cmsRepository.DeleteUser(Id);

            //ErrorMessage message = new ErrorMessage
            //{
            //    Title = "Информация",
            //    Info = "Запись удалена",
            //    Buttons = new ErrorMessageBtnModel[]
            //    {
            //        new ErrorMessageBtnModel { Url = $"{StartUrl}{Request.Url.Query}", Text = "ок", Action = "false" }
            //    }
            //};

            //model.ErrorInfo = message;

            return RedirectToAction("index");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, bool enabled, string size)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "searchtext", searchtext);
            query = AddFilterParam(query, "disabled", (!enabled).ToString().ToLower());
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);

            return Redirect(StartUrl + query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }

        /// <summary>
        /// Возвращает дерево фильтрации
        /// </summary>
        /// <returns></returns>
        private FilterTreeModel GetFilterTree()
        {
            if (model.Roles != null)
            {
                string link = Request.Url.Query;
                string editGroupUrl = "/admin/services/groupclaims/";
                string alias = "group";
                string active = Request.QueryString[alias];
                return new FilterTreeModel()
                {
                    Title = "Группы пользователей",
                    Icon = "icon-th-list-3",
                    //Url = editGroupUrl,
                    IsReadOnly = false,
                    //AccountGroup = (model.Account != null) ? model.Account.Group : "",
                    Items = model.Roles.Select(p =>
                        new CatalogList()
                        {
                            Title = p.Name,
                            Alias = p.Id.ToString(),
                            Link = AddFilterParam(link, alias, p.Id.ToString()),
                            Url = $"{editGroupUrl}{p.Id}",
                            IsSelected = active == p.Id.ToString()
                        }).ToArray(),
                    Link = "/admin/admins"
                };
            }
            return null;
        }

    }
}