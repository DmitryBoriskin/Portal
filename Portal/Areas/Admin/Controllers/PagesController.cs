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
            if (ModelState.IsValid)
            {
                backModel.Item.Id = id;
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

            model.Item = _cmsRepository.GetPage(id);
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
            _cmsRepository.DeletePage(Id);

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