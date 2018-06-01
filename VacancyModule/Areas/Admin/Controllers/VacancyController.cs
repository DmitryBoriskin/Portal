using PgDbase.entity;
using Portal.Areas.Admin;
using Portal.Areas.Admin.Models;
using System;
using System.Web;
using System.Web.Mvc;
using VacancyModule.Areas.Admin.Models;

namespace VacancyModule.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("Vacancy")]
    public class VacancyController : BeCoreController
    {
        FilterModel filter;
        VacancyViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            model = new VacancyViewModel()
            {
                PageName = PageName,
                DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                UserResolution = UserResolutionInfo
            };

            if (AccountInfo != null)
            {
                model.Menu = MenuCmsCore;
                model.MenuModul = MenuModulCore;
            }
        }

        // GET: Admin/Vacancy
        [Route]
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List = _cmsRepository.GetVacancies(filter);
            return View(model);
        }

        [Route, HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);

            return Redirect($"{StartUrl}item/{Guid.NewGuid()}/{query}");
        }

        [Route, HttpPost]
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

        [Route, HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }

        [Route("item/{id:guid}"), HttpGet]
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetVacancy(id);
            return View("Item", model);
        }

        [Route("item/{id:guid}"), HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, VacancyViewModel backModel)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            if (ModelState.IsValid)
            {
                backModel.Item.Id = id;
                if (_cmsRepository.CheckVacancyExists(id))
                {
                    _cmsRepository.UpdateVacancy(backModel.Item);
                    message.Info = "Запись обновлена";
                }
                else
                {
                    _cmsRepository.InsertVacancy(backModel.Item);
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

            model.Item = _cmsRepository.GetVacancy(id);
            model.ErrorInfo = message;
            return View("Item", model);
        }

        [Route("item/{id:guid}"), HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect($"{StartUrl}{Request.Url.Query}");
        }

        [Route("item/{id:guid}"), HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            bool result = _cmsRepository.DeleteVacancy(id);
            if (result)
            {
                message.Info = "Запись удалена";
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = $"{StartUrl}{Request.Url.Query}", Text = "ок", Action = "false" }
                };
            }

            model.ErrorInfo = message;
            return RedirectToAction("Index");
        }
    }
}