﻿using Portal.Areas.Admin.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class MenuController : BeCoreController
    {
        //public MenuController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //  : base(userManager, signInManager)
        //{ }

        MenuViewModel model;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new MenuViewModel
            {
                SiteId = SiteId,
                PageName = PageName,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                MenuCMS = MenuCmsCore,
                MenuModules = MenuModulCore,
            };
           
            model.MenuGroups = _cmsRepository.GetCmsMenu();
            //ViewBag.StartUrl = StartUrl;
            ViewBag.Title = "Структура CMS";
        }



        // GET: Admin/Menu
        public ActionResult Index()
        {
            model.MenuList = _cmsRepository.GetCmsMenuItems();
            return View(model);
        }


        //GET: Admin/Menu/item/{GUID}
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetCmsMenuItem(id);
            

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
        public ActionResult Save(Guid id,MenuViewModel backModel)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            if (ModelState.IsValid)
            {
                backModel.Item.Id = id;
                if (_cmsRepository.CheckMenuExist(id))
                {
                    _cmsRepository.UpdateMenu(backModel.Item);
                    message.Info = "Запись обновлена";
                }
                else
                {
                    _cmsRepository.InsertMenu(backModel.Item);
                    message.Info = "Запись добавлена";
                }
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = StartUrl + Request.Url.Query, Text = "вернуться в список" },
                    new ErrorMessageBtnModel { Url = StartUrl+"item/"+id, Text = "ок", Action = "false" }
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
            model.ErrorInfo = message;
            return View("item", model);
        }



        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            _cmsRepository.DeleteMenu(id);
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

    }
}