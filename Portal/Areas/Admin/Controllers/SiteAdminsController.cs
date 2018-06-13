﻿using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    [Authorize(Roles = "Developer,PortalAdmin,SiteAdmin")] 
    public class SiteAdminsController : BeCoreController
    {

        //Работать с администраторами в рамках сайта могут только Developer, PortalAdmin и SiteAdmin

        UsersViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new UsersViewModel()
            {
                SiteId = SiteId,
                PageName = PageName,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                MenuCMS = MenuCmsCore,
                MenuModules = MenuModulCore
            };
            
            model.Roles = _cmsRepository.GetRoles();
        }

        // GET: Admin/Users
        public ActionResult Index()
        {
            filter = GetFilter();
            model.Filter = GetFilterTree();
            var userFilter = FilterModel.Extend<UserFilter>(filter);

            model.List = _cmsRepository.GetSiteAdmins(userFilter);

            return View(model);
        }

        // GET: Admin/Users/<id>
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetUser(id);
            model.Item.Roles = _cmsRepository.GetUserRoles(id);
            return View("Item", model);
        }

        public ActionResult Insert()
        {
            filter = GetFilter();
            //string query = HttpUtility.UrlDecode(Request.Url.Query);
            //query = AddFilterParam(query, "page", String.Empty);

            //return Redirect($"{StartUrl}item/{Guid.NewGuid()}/{query}");
            //ViewBag.Users = _cmsRepository.GetSiteUsers(filter).Items.ToArray();

            return View("Part/FindUser", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, UsersViewModel backModel)
        {
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
            return Redirect($"{StartUrl}{Request.Url.Query}");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid Id)
        {
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
                string editGroupUrl = "/admin/roles/item/";
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
                            Title = p.Desc,
                            Alias = p.Name.ToString(),
                            Link = AddFilterParam(link, alias, p.Name),
                            Url = $"{editGroupUrl}{p.Id}",
                            IsSelected = active == p.Name.ToString()
                        }).ToArray(),
                    Link = "/admin/siteadmins/"
                };
            }
            return null;
        }

        [HttpPost]
        public ActionResult AddNewAdmin(Guid userId, string role)
        {
            var res = UserManager.AddToRole(userId.ToString(), role);
            if (res.Succeeded)
                return Json("Success");

            return Json("An Error Has Occourred");
        }
    }
}