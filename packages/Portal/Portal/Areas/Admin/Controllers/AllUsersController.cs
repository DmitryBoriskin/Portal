using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class AllUsersController : CoreController
    {
        UsersViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new UsersViewModel()
            {
                PageName = PageName,
                //DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Groups = _cmsRepository.GetGroups()
            };
            if (AccountInfo != null)
            {
                model.Menu = MenuCmsCore;
                model.MenuModul = MenuModulCore;
            }
        }

        // GET: Admin/Groups
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List = _cmsRepository.GetAllUsers(filter);
            model.Filter = GetFilterTree();
            return View(model);
        }

        // GET: Admin/Users/<id>
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetUser(id);
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
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            if (ModelState.IsValid)
            {
                backModel.Item.Id = id;
                if (_cmsRepository.CheckUserExists(id))
                {
                    _cmsRepository.UpdateUser(backModel.Item);
                    message.Info = "Запись обновлена";
                }
                else if (_cmsRepository.CheckUserExists(backModel.Item.Email))
                {
                    message.Info = "Пользователь с таким Email адресом уже существует";
                }
                else
                {
                    char[] _pass = backModel.Password.Password.ToCharArray();
                    Cripto password = new Cripto(_pass);
                    string NewSalt = password.Salt;
                    string NewHash = password.Hash;

                    backModel.Item.Hash = NewHash;
                    backModel.Item.Salt = NewSalt;

                    _cmsRepository.InsertUser(backModel.Item);

                    message.Info = "Запись добавлена";
                }
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = StartUrl + Request.Url.Query, Text = "вернуться в список" },
                    new ErrorMessageBtnModel { Url = $"{StartUrl}/item/{id}", Text = "ок", Action = "false" }
                };
            }
            else
            {
                message.Info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом";
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = $"{StartUrl}/item/{id}", Text = "ок", Action = "false" }
                };
            }

            model.Item = _cmsRepository.GetUser(id);
            model.ErrorInfo = message;
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
            _cmsRepository.DeleteUser(Id);

            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация",
                Info = "Запись удалена",
                Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = $"{StartUrl}{Request.Url.Query}", Text = "ок", Action = "false" }
                }
            };

            model.ErrorInfo = message;

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
            if (model.Groups != null)
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
                    Items = model.Groups.Select(p =>
                        new CatalogList()
                        {
                            Title = p.Title,
                            Alias = p.Id.ToString(),
                            Link = AddFilterParam(link, alias, p.Id.ToString()),
                            Url = $"{editGroupUrl}{p.Id}",
                            IsSelected = active == p.Id.ToString()
                        }).ToArray(),
                    Link = "/admin/allusers"
                };
            }
            return null;
        }
    }
}