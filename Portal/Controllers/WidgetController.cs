using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PgDbase.entity;
using PgDbase.Repository.front;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Portal.Controllers.AccountManageController;

namespace Portal.Controllers
{
    public class WidgetController : CoreController
    {
        public ActionResult PageGroup(string alias, string view)
        {

            List<PageModel> model = _Repository.GetPageGroup(alias);


            if (String.IsNullOrEmpty(view))
            {
                return PartialView(model);
            }
            else return PartialView(view, model);
        } 


        public ActionResult SelectSubscr(Guid SubscrId)
        {
            _Repository.SelectSubscr(SubscrId, Guid.Parse(User.Identity.GetUserId()));
            return Json("success");
        }


        #region Изьменение данных пользователя

        [ChildActionOnly]
        public PartialViewResult GetChangeUserInfo()
        {
            var sub_model =_Repository.GetUser(Guid.Parse(User.Identity.GetUserId()));
            UserModelUpdate model = new UserModelUpdate
            {
                Surname = sub_model.Surname,
                Name = sub_model.Name,
                Patronimyc = sub_model.Patronimyc,
                Phone = sub_model.Phone,
                Email = sub_model.Email
            };
            return PartialView("GetChangeUserInfo", model);
        }
        [HttpPost]
        public async Task<ActionResult> ChangeUserInfo(UserModelUpdate model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                user.UserInfo.Surname = model.Surname;
                user.UserInfo.Name = model.Name;
                user.UserInfo.Patronymic = model.Patronimyc;
                user.PhoneNumber = model.Phone;
                user.Email = model.Email;
                var updateUserResult = await UserManager.UpdateAsync(user);

                return RedirectToAction("Index", "Settings", new { Message = ManageMessageId.ChangeUserInfo });
            }
            else
            {   
                return RedirectToAction("Index", "Settings", new { Message = ManageMessageId.Error });
            }

        }
        #endregion

        #region смена пароля
        [ChildActionOnly]
        public PartialViewResult GetChangePassword()
        {
            return PartialView("GetChangePassword", new ChangePasswordFrontModel { });
        }
        // POST: /Settings/ChangePassword
        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordFrontModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index","Settings", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            return RedirectToAction("Index","Settings", new { Message = ManageMessageId.Error });
        }
        #endregion

    }
}