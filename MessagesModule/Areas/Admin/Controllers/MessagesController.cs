using Portal.Areas.Admin.Controllers;
using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MessagesModule.Areas.Admin.Models;
using Portal.Areas.Admin.Models;
using PgDbase.Entity.modules.messages;

namespace MessagesModule.Areas.Admin.Controllers
{
    public class MessagesController : BeCoreController
    {
        MessagesViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);            

            //Есть ли у сайта доступ к модулю
            if (!_cmsRepository.ModuleAllowed(ControllerName))
                Response.Redirect("/Admin/");

            model = new MessagesViewModel()
            {
                SiteId = SiteId,
                PageName = "Сообщения",
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                MenuCMS = MenuCmsCore,
                MenuModules = MenuModulCore,
            };
        }
        // GET: Admin/Messages
        public ActionResult Index()
        {
            filter = GetFilter();
            var mfilter = FilterModel.Extend<MessagesFilter>(filter);
            model.List = _cmsRepository.GetMessages(filter);
            return View(model);
        }
        public ActionResult Item(Guid id)
        {
            model.MessageHistory = _cmsRepository.GetMessagesThemeItem(id);

            List<UserModel> SlectList = new List<UserModel>();
            SlectList.Add(new UserModel { });
            SlectList.AddRange(_cmsRepository.GetUserList());
                
            model.UserList=SlectList.Select(s => new SelectListItem { Text = s.Surname + " " + s.Name + " " + s.Patronimyc,Value=s.Id.ToString()});

            if (model.MessageHistory != null && model.MessageHistory.Count() > 0)
            {
                model.Item = new MessagesModel
                {
                    Theme = model.MessageHistory.Last().Theme
                };                    
            }
            return View(model);
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
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, bool viewmsg,  string size, DateTime? datestart, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "searchtext", searchtext);

            query = AddFilterParam(query, "viewmsg", (!viewmsg).ToString().ToLower());

            if (datestart.HasValue)
                query = AddFilterParam(query, "datestart", datestart.Value.ToString("dd.MM.yyyy").ToLower());
            
            if (dateend.HasValue)
                query = AddFilterParam(query, "dateend", dateend.Value.ToString("dd.MM.yyyy").ToLower());
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);

            return Redirect(StartUrl + query);
        }


        [HttpPost]        
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, MessagesViewModel backModel)
        {   
            if (ModelState.IsValid)
            {
                backModel.Item.Date = DateTime.Now;
                backModel.Item.Admin = true;
                if (!_cmsRepository.CheckMessages(id))
                {
                    //новая тема                    
                    backModel.Item.Id = id;
                }
                else
                {
                    //новое сообщение в существующей теме
                    backModel.Item.Id = Guid.NewGuid();
                    backModel.Item.ParentId = id;
                }
                _cmsRepository.InsertMessages(backModel.Item);
                return Redirect("/admin/messages/item/" + id);
            }
            else
            {
                #region error no valid
                ErrorMessage message = new ErrorMessage
                {
                    Title = "Информация"
                };
                message.Info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом";
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = "#", Text = "ок", Action = "false" }
                };
                model.ErrorInfo = message;
                #endregion

            }
            
            return View("item", model);
        }


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            _cmsRepository.DeleteMessages(id);
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