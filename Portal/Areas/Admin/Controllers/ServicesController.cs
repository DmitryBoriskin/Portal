﻿using Microsoft.AspNet.Identity;
using PgDbase.entity;
using Portal.Areas.Admin.Models;
using Portal.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class ServicesController : BeCoreController
    {
        //public ServicesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //   : base(userManager, signInManager) { }


        public ActionResult Log(Guid id, string type)
        {
            LogModel[] model = null;
            switch (type)
            {
                case "page":
                    model = _cmsRepository.GetPageLogs(id);
                    break;
                case "user":
                    model = _cmsRepository.GetUserLogs(id);
                    break;
            }

            return PartialView("Log", model);
        }

        public void ForgotPassword(Guid id)
        {
            UsersViewModel model = new UsersViewModel()
            {
                Item = _cmsRepository.GetUser(id)
            };

            string codeGenerated = UserManager.GeneratePasswordResetToken(id.ToString());
            var callbackUrl = Url.Action("ResetPassword", "Services", new { userId = id.ToString(), code = codeGenerated }, protocol: Request.Url.Scheme);

            Response.Redirect(callbackUrl);
               //PartialView("ChangePass", model);
        }

        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            ViewBag.UserEmail = UserManager.FindById(userId).Email;
            return code == null ? PartialView("Error") : PartialView("");
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var siteId = SiteId;
            if (siteId == Guid.Empty)
                return RedirectToAction("Error", "Main");

            var getUser = UserManager.Users.Where(u => u.Email == model.Email && u.SiteId == siteId).SingleOrDefault();

            if (getUser == null)
                return RedirectToAction("ResetPasswordConfirmation", "Services");

            var userId = getUser.Id;


            var user = UserManager.FindById(userId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Services");
            }
            var result = UserManager.ResetPassword(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Services");
            }

            return View("");
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View("");
        }



        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "password-update")]
        public ActionResult ChangePass(Guid id, UsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                string NewPass = model.Password.Password;
                Cripto pass = new Cripto(NewPass.ToCharArray());
                string NewSalt = pass.Salt;
                string NewHash = pass.Hash;
               
                //_cmsRepository.ChangePassword(id, NewSalt, NewHash);
                ViewBag.SuccesAlert = "Пароль изменен";
            }

            model = new UsersViewModel()
            {
                Item = _cmsRepository.GetUser(id)
            };

            return PartialView("ChangePass", model);
        }

        public ActionResult AddFilterTree(string section, string id)
        {
            var model = new GroupsModel();
            if (id != null)
            {
                switch (section)
                {
                    case "pages":
                        model = _cmsRepository.GetPageGroup(Guid.Parse(id));
                        break;
                }
            }
            model.Alias = section;
            return PartialView("FilterTreeItem", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-filter-tree-btn")]
        public ActionResult SaveFilterTreeItem(GroupsModel item)
        {
            if (ModelState.IsValid)
            {
                if (item.Id == Guid.Empty)
                {
                    item.Id = Guid.NewGuid();
                }
                switch (item.Alias)
                {
                    case "pages":
                        _cmsRepository.SavePageGroup(item);
                        break;
                }
            }
            return Redirect($"/admin/{item.Alias}");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-filter-tree-btn")]
        public ActionResult DeleteFilterTreeItem(GroupsModel item)
        {
            switch (item.Alias)
            {
                case "pages":
                    _cmsRepository.DeletePageGroup(item.Id);
                    break;
            }
            return Redirect($"/admin/{item.Alias}");
        }


        [HttpPost]
        public ActionResult ChangePosition(string group, string menusort, Guid id, int position)
        {
            bool result = false;
            switch (group.ToLower())
            {
                case "cmsmenu":
                    _cmsRepository.ChangePositionMenu(id, position);
                    break;
                case "pages":
                    result = _cmsRepository.ChangePositionPages(id, position);
                    break;
            }
            return Content(result.ToString());
        }

        [HttpPost]
        public ActionResult ChangePositionAlbum(Guid album, Guid id, int permit)
        {
            bool result = _cmsRepository.ChangePositionPhoto(album, id, permit);
            return Content(result.ToString());
        }

    }
}