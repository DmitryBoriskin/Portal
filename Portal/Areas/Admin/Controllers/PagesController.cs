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
            filter = GetFilter();
            var mfilter = FilterModel.Extend<PageFilterModel>(filter);
            model.List = _cmsRepository.GetPages(mfilter);
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
        public ActionResult Item(Guid id, PageViewModel backModel)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };

            string _parent = backModel.Item.ParentId != Guid.Empty ? $"item/{backModel.Item.ParentId.ToString()}" : "";
            string backToListUrl = $"{StartUrl}{_parent}{Request.Url.Query}";

            if (ModelState.IsValid)
            {
                backModel.Item.Id = id;
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
                message.Info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом";
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
    }
}