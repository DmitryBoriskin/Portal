using Newtonsoft.Json;
using PgDbase.entity;
using Portal.Areas.Admin.Models;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    [Authorize(Roles = "Developer,PortalAdmin,SiteAdmin")]
    public class SiteUsersController : BeCoreController
    {
        // С пользователями могут работать только роли Developer, PortalAdmin, SiteAdmin

        UsersViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new UsersViewModel()
            {
                PageName = PageName,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
            };
            if (AccountInfo != null)
            {
                model.Menu = MenuCmsCore;
                model.MenuModules = MenuModulCore;
            }
        }

        // GET: Admin/Users
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List = _cmsRepository.GetSiteUsers(filter);
            return View(model);
        }

        // GET: Admin/Users/<id>
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetUser(id);
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

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public async Task<ActionResult> Save(Guid id, UsersViewModel backModel)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            if (ModelState.IsValid)
            {
                backModel.Item.Id = id;
                var _userId = backModel.Item.Id.ToString();

                var user = new ApplicationUser
                {
                    Id = _userId,
                    UserName = backModel.Item.Id.ToString(),
                    Email = backModel.Item.Email,
                    EmailConfirmed = backModel.Item.EmailConfirmed,
                    PhoneNumber = backModel.Item.Phone,
                    PhoneNumberConfirmed = backModel.Item.PhoneConfirmed,
                    SiteId = SiteId,
                    UserId = backModel.Item.Id,
                    UserInfo = new UserProfile()
                    {
                        UserId = _userId,
                        Surname = backModel.Item.Surname,
                        Name = backModel.Item.Name,
                        Patronymic = backModel.Item.Patronimyc,
                        BirthDate = backModel.Item.BirthDate,
                        RegDate = DateTime.Now
                    }
                };

               
                if (_cmsRepository.CheckUserExists(id))
                {
                   
                    user = await UserManager.FindByIdAsync(_userId);
                    user.Email = backModel.Item.Email;
                    user.EmailConfirmed = backModel.Item.EmailConfirmed;
                    user.PhoneNumber = backModel.Item.Phone;
                    user.PhoneNumberConfirmed = backModel.Item.PhoneConfirmed;

                    user.UserInfo.Surname = backModel.Item.Surname;
                    user.UserInfo.Name = backModel.Item.Name;
                    user.UserInfo.Patronymic = backModel.Item.Patronimyc;
                    user.UserInfo.BirthDate = backModel.Item.BirthDate;

                    if ((user.Email == backModel.Item.Email) || !_cmsRepository.CheckUserExists(backModel.Item.Email, SiteId))
                    {
                        // _cmsRepository.UpdateUser(backModel.Item);
                        var updateUserResult = await UserManager.UpdateAsync(user);
                        //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        var userlogOff = await UserManager.UpdateSecurityStampAsync(_userId);

                        if (updateUserResult.Succeeded)
                            message.Info = "Запись обновлена";
                        else
                            message.Info = "Произошла ошибка";
                    }
                    else
                    {
                        message.Info = "Пользователь с таким Email уже существует на этом сайте";
                    }
                }
                else
                {
                    //_cmsRepository.InsertUser(backModel.Item);
                    // var res = UserManager.AddPasswordAsync(id.ToString(), backModel.Password.Password);
                    if (_cmsRepository.CheckUserExists(backModel.Item.Email, SiteId))
                    {
                        message.Info = "Пользователь с таким Email уже существует на этом сайте";
                    }
                    else
                    {
                        var addUserResult = await UserManager.CreateAsync(user, backModel.Password.Password);

                        if (addUserResult.Succeeded)
                        {
                            var addRolesResult = await UserManager.AddToRolesAsync(id.ToString(), new string[] { "User", SiteId.ToString() });

                            string text = "<p>Уважаемый " + user.UserInfo.FullName + ", администратором по вашей просьбе была создана учетная запись на сайте " + Request.Url.Host + ".</p>";
                            text += "<p>Если у вас возникли вопросы, вам необходимо обратиться в службу поддержки компании.</p>";
                            text += "<p>С уважением, администрация сайта!</p>";
                            text += "<hr><i><span style=\"font-size:11px\">Данное сообщение отправлено роботом, на него не нужно отвечать</i></span>";

                            await UserManager.SendEmailAsync(user.Id, "Создание учетной записи", text);

                            message.Info = "Запись добавлена";
                        }
                        else
                            message.Info = "Произошла ошибка";
                    }

                }
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = StartUrl + Request.Url.Query, Text = "вернуться в список" },
                    new ErrorMessageBtnModel { Url = $"{StartUrl}/item/{id}", Text = "ок", Action = "false" }
                };
            }
            else
            {
                message.Info = "Ошибка в заполнении формы. Пожалуйста, заполните все поля верно!";
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = $"{StartUrl}/item/{id}", Text = "ок", Action = "false" }
                };
            }

            model.Item = _cmsRepository.GetUser(id);
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
        public async Task<ActionResult> Delete(Guid Id)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };

            //_cmsRepository.DeleteUser(Id);

            var userlogOff = await UserManager.UpdateSecurityStampAsync(Id.ToString());

            if (userlogOff.Succeeded)
            {

                var user = await UserManager.FindByIdAsync(Id.ToString());
                var deleteUserResult = await UserManager.DeleteAsync(user);

                if (deleteUserResult.Succeeded)
                {
                    string text = "<p>Уважаемый " + user.UserInfo.FullName + ", Ваша учетная запись на сайте " + Request.Url.Host + " была удалена администратором.</p>";
                    text += "<p>Если у вас возникли вопросы, вам необходимо обратиться в службу поддержки компании.</p>";
                    text += "<p>С уважением, администрация сайта!</p>";
                    text += "<hr><i><span style=\"font-size:11px\">Данное сообщение отправлено роботом, на него не нужно отвечать</i></span>";

                    await UserManager.SendEmailAsync(user.Id, "Удаление учетной записи", text);

                    message.Info = "Запись удалена";
                }
                else
                    message.Info = "Произошла ошибка";
            }
            else
                message.Info = "Не удалось разлогинить пользователя. Пользователь не удален!";


            message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = $"{StartUrl}{Request.Url.Query}", Text = "ок", Action = "false" }
                };

            model.ErrorInfo = message;

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

        [HttpPost]
        public string List (string query)
        {
            var users = _cmsRepository.GetSiteUsersList(query);
            var data = users.Select(t => new { id = t.Id, text = $"{t.FullName} ({t.Email})" });

            return JsonConvert.SerializeObject(data);
        }

    }
}