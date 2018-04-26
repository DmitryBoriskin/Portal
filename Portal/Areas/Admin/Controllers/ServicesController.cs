using Portal.Areas.Admin.Models;
using System;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class ServicesController : CoreController
    {
        public ActionResult ChangePass(Guid id)
        {
            UsersViewModel model = new UsersViewModel()
            {
                //UserResolution = UserResolutionInfo,
                Item = _cmsRepository.GetUser(id)
            };
            return PartialView("ChangePass", model);
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
                //UserResolution = UserResolutionInfo,
                Item = _cmsRepository.GetUser(id)
            };

            return PartialView("ChangePass", model);
        }
    }
}