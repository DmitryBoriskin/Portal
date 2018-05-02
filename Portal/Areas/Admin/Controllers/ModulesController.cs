﻿using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin
{
    public class ModulesController : CoreController
    {
        ModuleViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new ModuleViewModel()
            {
                PageName = "Модули",
                DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };

            if (AccountInfo != null)
                model.Menu = _cmsRepository.GetCmsMenu(AccountInfo.Id);
        }

        // GET: Admin/Users
        public ActionResult Index()
        {
            filter = GetFilter();
            var mfilter = FilterModel.Extend<ModuleFilter>(filter);
            model.List = _cmsRepository.GetModules(mfilter);

            ViewBag.SearchText = filter.SearchText;
            return View(model);
        }

        // GET: Admin/Users/<id>
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetModule(id);
            model.Templates = _cmsRepository.GetTemplatesList()
                                .Where(t => t.Controller.Id == Guid.Empty || t.Controller.Id == model.Item.Id)
                                .ToArray();
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
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, ModuleViewModel backModel)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            if (ModelState.IsValid)
            {
                backModel.Item.Id = id;

                    if (_cmsRepository.ModuleExists(id))
                    {
                        _cmsRepository.UpdateModule(backModel.Item);
                        message.Info = "Запись обновлена";
                    }
                    else
                    {
                        _cmsRepository.InsertModule(backModel.Item);

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

            //model.Item = _cmsRepository.GetUser(id);
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
            _cmsRepository.DeleteModule(Id);
            
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
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, string group, string size)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "searchtext", searchtext);
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
    }
}