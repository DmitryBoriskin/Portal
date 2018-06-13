using LkModule.Areas.Admin.Models;
using PgDbase.entity;
using Portal.Areas.Admin;
using Portal.Areas.Admin.Controllers;
using Portal.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Controllers
{
    public class SubscrsController : BeCoreController
    {
        FilterModel filter;
        SubscrViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_cmsRepository.ModuleAllowed(ControllerName))
                Response.Redirect("/Admin/");

            model = new SubscrViewModel()
            {
                SiteId = SiteId,
                PageName = PageName,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                MenuCMS = MenuCmsCore,
                MenuModules = MenuModulCore,

                Departments = new SelectList(_cmsRepository.GetDepartments()
                    .Select(s => new SelectListItem
                    {
                        Text = s.Title,
                        Value = s.Id.ToString()
                    }).ToArray(), "Value", "Text")
            };
        }

        // GET: Admin/Lk
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List = _cmsRepository.GetSubscrs(filter);
            ViewBag.Department = Request.Params["category"];
            return View(model);
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
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, bool enabled, string size, string department)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "searchtext", searchtext);
            query = AddFilterParam(query, "disabled", (!enabled).ToString().ToLower());
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);
            query = AddFilterParam(query, "category", department);

            return Redirect(StartUrl + query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect($"{StartUrl}{Request.Url.Query}");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            bool result = _cmsRepository.DeleteSubscr(id);
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

        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetSubscr(id);
            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, SubscrViewModel backModel)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            if (ModelState.IsValid)
            {
                backModel.Item.Id = id;
                if (_cmsRepository.CheckSubscrExists(id))
                {
                    _cmsRepository.UpdateSubscr(backModel.Item);
                    message.Info = "Запись обновлена";
                }
                else
                {
                    _cmsRepository.InsertSubscr(backModel.Item);
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

            model.Item = _cmsRepository.GetSubscr(id);
            model.ErrorInfo = message;
            return View("Item", model);
        }
    }
}