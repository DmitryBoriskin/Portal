using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class ServicesController : CoreController
    {
        //public ActionResult ChangePass(Guid id)
        //{
        //    UsersViewModel model = new UsersViewModel()
        //    {
        //        //UserResolution = UserResolutionInfo,
        //        Item = _cmsRepository.GetUser(id)
        //    };
        //    return PartialView("ChangePass", model);
        //}

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "password-update")]
        //public ActionResult ChangePass(Guid id, PortalUsersViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string NewPass = model.Password.Password;

        //        Cripto pass = new Cripto(NewPass.ToCharArray());
        //        string NewSalt = pass.Salt;
        //        string NewHash = pass.Hash;
        //        _cmsRepository.changePassword(id, NewSalt, NewHash); //, AccountInfo.id, RequestUserInfo.IP
        //        ViewBag.SuccesAlert = "Пароль изменен";
        //        #region оповещение на e-mail
        //        //string ErrorText = "";
        //        //string Massege = String.Empty;
        //        //Mailer Letter = new Mailer();
        //        //Letter.Theme = "Изменение пароля";


        //        //bool sex = true;
        //        //if (model.User.B_Sex.HasValue) { if (!(bool)model.User.B_Sex) sex = false; }
        //        //Massege = (sex) ? "<p>Уважаемый " : "<p>Уважаемая ";
        //        //Massege += model.User.C_Surname + " " + model.User.C_Name + "</p>";
        //        //Massege += "<p>Ваш пароль на сайте <b>" + model.Settings.Title + "</b> был изменен</p>";
        //        //Massege += "<p>Ваш новый пароль:<b>" + NewPass + "</b></p>";
        //        //Massege += "<p>С уважением, администрация портала!</p>";
        //        //Massege += "<hr><span style=\"font-size:11px\">Это сообщение отпралено роботом, на него не надо отвечать</span>";
        //        //Letter.MailTo = model.User.C_EMail;
        //        //Letter.Text = Massege;
        //        //ErrorText = Letter.SendMail();
        //        #endregion
        //    }

        //    model = new PortalUsersViewModel()
        //    {
        //        UserResolution = UserResolutionInfo,
        //        Item = _cmsRepository.getUser(id)

        //    };

        //    return PartialView("ChangePass", model);
        //}
    }
}