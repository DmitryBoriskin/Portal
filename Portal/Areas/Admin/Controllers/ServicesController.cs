using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
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
                _cmsRepository.ChangePassword(id, NewSalt, NewHash);
                ViewBag.SuccesAlert = "Пароль изменен";
            }

            model = new UsersViewModel()
            {
                //UserResolution = UserResolutionInfo,
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

        //[HttpGet]
        //public ActionResult GroupClaims(string id)
        //{
        //    UserGroupResolution[] model = null;
            
        //    if (!String.IsNullOrWhiteSpace(id))
        //    {
        //        Guid groupId = Guid.Parse(id);
        //        model = _cmsRepository.GetGroupResolutions(groupId);
        //    }
        //    return PartialView("GroupClaims", model);
        //}

        //[HttpPost]
        //public ActionResult UpdateGroupClaims(ClaimParams data)
        //{
        //    var res = _cmsRepository.UpdateGroupResolution(data);
        //    if (res)
        //    {
        //        return Json("Success");
        //    }
        //    return Json("An Error Has occourred");
        //}
    }
}