using Microsoft.AspNet.Identity;
using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class RolesController : BeCoreController
    {
        //public UsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //    : base(userManager, signInManager) { }

        RoleViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new RoleViewModel()
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
        }

        // GET: Admin/Roles
        public ActionResult Index()
        {
            filter = GetFilter();

            //Исключаем из выборки вышестоящие роли, например SiteAdmin не должен видеть Developer и PortalAdmin
            string[] excludeRoles = null;

            if (User.IsInRole("PortalAdmin"))
                excludeRoles = new string[] { "Developer" };

            else if (User.IsInRole("SiteAdmin"))
                excludeRoles = new string[] { "Developer", "PortalAdmin" };
            //else developer

            model.List = _cmsRepository.GetRoles(excludeRoles);
            return View(model);
        }

        // GET: Admin/Users/<id>
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetRole(id);
            model.Modules = _cmsRepository.GetModulesList();

            //Права на модули
            if(model.Modules.Count() > 0)
            {
                foreach(var module in model.Modules)
                {
                    module.RoleModuleClaims = new RoleModuleClaims()
                    {
                        View = true,
                        Edit = true,
                        Create = true,
                        Delete = true
                    };
                    if(module.ModuleParts.Count()>0)
                    {
                        foreach(var part in module.ModuleParts.Distinct())
                        {
                            if (!model.Item.Claims.Any(t => t.Type == part.ControllerName && t.Value == "view"))
                                module.RoleModuleClaims.View = false;
                            if (!model.Item.Claims.Any(t => t.Type == part.ControllerName && t.Value == "edit"))
                                module.RoleModuleClaims.Edit = false;
                            if (!model.Item.Claims.Any(t => t.Type == part.ControllerName && t.Value == "create"))
                                module.RoleModuleClaims.Create = false;
                            if (!model.Item.Claims.Any(t => t.Type == part.ControllerName && t.Value == "delete"))
                                module.RoleModuleClaims.Delete = false;
                        }
                    }
                }
            }

            //Потом добавить фильтрацию по доступным сайтам
            //model.Item.Sites = _cmsRepository.GetSites();
            model.Item.Sites = model.Sites
                .Where(s => model.Item.Claims.Any(c => c.Type.ToLower() == "_siteidentity" && c.Value.ToLower() == s.Id.ToString().ToLower()))
                .ToArray();

            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);
            
            return Redirect($"{StartUrl}item/{Guid.NewGuid()}/{query}");
        }

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        //public ActionResult Save(Guid id, UsersViewModel backModel)
        //{
        //    ErrorMessage message = new ErrorMessage
        //    {
        //        Title = "Информация"
        //    };
        //    if (ModelState.IsValid)
        //    {
        //        backModel.Item.Id = id;
        //        if (_cmsRepository.CheckUserExists(id))
        //        {
        //            _cmsRepository.UpdateUser(backModel.Item);
        //            message.Info = "Запись обновлена";
        //        }
        //        else if (_cmsRepository.CheckUserExists(backModel.Item.Email))
        //        {
        //            message.Info = "Пользователь с таким Email адресом уже существует";
        //        }
        //        else
        //        {
        //            char[] _pass = backModel.Password.Password.ToCharArray();
        //            Cripto password = new Cripto(_pass);
        //            string NewSalt = password.Salt;
        //            string NewHash = password.Hash;

        //            backModel.Item.Hash = NewHash;
        //            backModel.Item.Salt = NewSalt;

        //            _cmsRepository.InsertUser(backModel.Item);

        //            message.Info = "Запись добавлена";
        //        }
        //        message.Buttons = new ErrorMessageBtnModel[]
        //        {
        //            new ErrorMessageBtnModel { Url = StartUrl + Request.Url.Query, Text = "вернуться в список" },
        //            new ErrorMessageBtnModel { Url = $"{StartUrl}/item/{id}", Text = "ок", Action = "false" }
        //        };
        //    }
        //    else
        //    {
        //        message.Info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом";
        //        message.Buttons = new ErrorMessageBtnModel[]
        //        {
        //            new ErrorMessageBtnModel { Url = $"{StartUrl}/item/{id}", Text = "ок", Action = "false" }
        //        };
        //    }

        //    model.Item = _cmsRepository.GetRole(id);
        //    model.ErrorInfo = message;
        //    return View("item", model);
        //}

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
        }

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        //public ActionResult Delete(Guid Id)
        //{
        //    _cmsRepository.DeleteRole(Id);

        //    ErrorMessage message = new ErrorMessage
        //    {
        //        Title = "Информация",
        //        Info = "Запись удалена",
        //        Buttons = new ErrorMessageBtnModel[]
        //        {
        //            new ErrorMessageBtnModel { Url = $"{StartUrl}{Request.Url.Query}", Text = "ок", Action = "false" }
        //        }
        //    };

        //    model.ErrorInfo = message;

        //    return RedirectToAction("index");
        //}

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
        /// Проставляем права у роли
        /// </summary>
        /// <param name="roleClaim"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateRoleClaim(RoleClaimModel roleClaim)
        {
            var res = true;

            if (roleClaim.Section == ClaimSection.CMS)
            {
                res = _cmsRepository.UpdateRoleClaim(roleClaim);
            }
            else if (roleClaim.Section == ClaimSection.Module)
            {
                //Ранее предполагалось, что компоненты модуля находятся в одном контроллере
                //Права на дочерние контроллеры (компоненты модуля)
                var module = _cmsRepository.GetModule(roleClaim.Type);
                if (module != null && module.ModuleParts.Count() > 0)
                {
                    foreach (var part in module.ModuleParts.Distinct())
                    {
                        var newClaim = new RoleClaimModel()
                        {
                            RoleId = roleClaim.RoleId,
                            Type = part.ControllerName,
                            Value = roleClaim.Value,
                            Checked = roleClaim.Checked
                        };
                        var updRes = _cmsRepository.UpdateRoleClaim(newClaim);
                        if (updRes)
                            res = false;
                    }
                }
            }
            else
                res = false;

            if (res)
                return Json("Success");

            return Json("An Error Has Occourred");
        }

        /// <summary>
        /// Добавляем пользователю текущего сайта роль
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUserToRole(Guid userId, string role)
        {

            var res = UserManager.AddToRole(userId.ToString(), role);

            if (res.Succeeded)
                return Json("Success");

            return Json("An Error Has Occourred");
        }

        /// <summary>
        /// Удаляем у пользователя текущего сайта роль
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteUserFromRole(Guid userId, string role)
        {

            var res = UserManager.RemoveFromRole(userId.ToString(), role);

            if (res.Succeeded)
               return Json("Success");

            return Json("An Error Has Occourred");
        }

        /// <summary>
        /// Добавляем админа на конкретный сайт с определенной ролью
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddUserToSiteRole(Guid userId, string role, Guid siteId)
        {

            //Если пользователь зарегистрирован на сайте, на котором назначаются права
            //то просто назначаем права
            var user = UserManager.FindById(userId.ToString());
            if(user != null)
            {
                if(user.SiteId == siteId)
                {
                    var res = UserManager.AddToRole(userId.ToString(), role);

                    if (res.Succeeded)
                        return Json("Success");

                    return Json("An Error Has Occourred");
                }
                else
                {
                    //Если назначаем права на другом сайте, то
                    //Дублируем аккаунт для указанного сайта

                    var newId = Guid.NewGuid(); 
                    var dublicateRes = _cmsRepository.DublicateUser(newId, userId, siteId);
                    //Задаем этому пользователю роль
                    //var res = UserManager.AddToRole(newId.ToString(), "User");
                    var res = UserManager.AddToRole(newId.ToString(), role);

                    if (res.Succeeded)
                        return Json("Success");

                    return Json("An Error Has Occourred");

                }
            }
            return Json("An Error Has Occourred");
        }

        
        [HttpPost]
        public ActionResult AddRoleToSite(Guid roleId, Guid siteId)
        {
            var roleClaim = new RoleClaimModel()
            {
                RoleId = roleId,
                Type = "_SiteIdentity",
                Value = siteId.ToString(),
                Checked = true
            };
            var res = _cmsRepository.UpdateRoleClaim(roleClaim);

            if (res)
                return Json("Success");

            return Json("An Error Has Occourred");
        }


        [HttpPost]
        public ActionResult DeleteRoleFromSite(Guid roleId, Guid siteId)
        {
            var roleClaim = new RoleClaimModel()
            {
                RoleId = roleId,
                Type = "_SiteIdentity",
                Value = siteId.ToString(),
                Checked = false
            };
            var res = _cmsRepository.UpdateRoleClaim(roleClaim);

            if (res)
                return Json("Success");

            return Json("An Error Has Occourred");
        }

    }
}