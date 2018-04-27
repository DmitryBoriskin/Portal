﻿using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
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

            /*model = new UsersViewModel
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
            }*/
        }

        // GET: Admin/Users
        public ActionResult Index()
        {
            filter = GetFilter();
            //model.List = _cmsRepository.GetUsers(filter);
            return View(model);
        }

        // GET: Admin/Users/<id>
        public ActionResult Item(Guid id)
        {
            //model.Item = _cmsRepository.GetUser(id);
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
        public ActionResult Save(Guid id, UsersViewModel backModel)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            if (ModelState.IsValid)
            {
                backModel.Item.Id = id;
                if (_cmsRepository.CheckUserExists(backModel.Item.Email))
                {
                    message.Info = "Пользователь с таким Email адресом уже существует";
                }
                else
                {
                    if (_cmsRepository.CheckUserExists(id))
                    {
                        _cmsRepository.UpdateUser(backModel.Item);
                        message.Info = "Запись обновлена";
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
                }
                message.Buttons = new ErrorMessageBtnModel[] 
                {
                    new ErrorMessageBtnModel { Url = StartUrl + Request.Url.Query, Text = "вернуться в список" },
                    new ErrorMessageBtnModel { Url = "#", Text = "ок", Action = "false" }
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
            _cmsRepository.DeleteUser(Id);
            
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
    }
}