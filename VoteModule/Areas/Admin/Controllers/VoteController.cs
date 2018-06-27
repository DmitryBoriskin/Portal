using PgDbase.entity;
using PgDbase.Entity.modules.vote;
using Portal.Areas.Admin.Controllers;
using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VoteModule.Areas.Admin.Models;

namespace VoteModule.Areas.Admin.Controllers
{
    public class VoteController : BeCoreController
    {
        VoteViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_cmsRepository.ModuleAllowed(ControllerName))
                Response.Redirect("/Admin/");

            model = new VoteViewModel()
            {
                SiteId = SiteId,
                PageName = "Голосования",
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                MenuCMS = MenuCmsCore,
                MenuModules = MenuModulCore,
            };
        }

        // GET: Admin/Vote
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List = _cmsRepository.GetVoteList(filter);
            return View(model);
        }
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetVoteItem(id);
            if (model.Item != null)
            {
                model.Answer = new AnswerModel
                {
                    ParentId = model.Item.Id
                };                    
            }            
            return View(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, VoteViewModel backModel)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            backModel.Item.Id = id;
            if (ModelState.IsValid)
            {
                if (_cmsRepository.ChechVote(id))
                {
                    message.Info = "Запись обновлена";
                    _cmsRepository.UpdateVote(backModel.Item);
                }
                else
                {
                    _cmsRepository.InsertVote(backModel.Item);
                    message.Info = "Запись добавлена";
                }
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = StartUrl + Request.Url.Query, Text = "вернуться в список" },
                    new ErrorMessageBtnModel { Url = StartUrl + "item/"+id, Text = "ок", Action = "false" }
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
            model.Item = _cmsRepository.GetVoteItem(id);
            if (model.Item != null)
            {
                model.Answer = new AnswerModel
                {
                    ParentId = model.Item.Id
                };
            }
            return View("item", model);
        }


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            _cmsRepository.DeleteVote(id);
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
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "add-new-answer")]        
        public ActionResult AddNewAnswer(Guid id, VoteViewModel backModel)
        {
            backModel.Answer.ParentId = id;
            _cmsRepository.AddAnswer(backModel.Answer);
            model.Item = _cmsRepository.GetVoteItem(id);
            model.Answer = new AnswerModel();
            return Redirect(StartUrl + "item/" + id);
            //return View("item", model);
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
        public ActionResult Search(string searchtext, bool enabled, string size, DateTime? datestart, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "searchtext", searchtext);
            query = AddFilterParam(query, "disabled", (!enabled).ToString().ToLower());

            if (datestart.HasValue)
                query = AddFilterParam(query, "datestart", datestart.Value.ToString("dd.MM.yyyy").ToLower());

            if (dateend.HasValue)
                query = AddFilterParam(query, "dateend", dateend.Value.ToString("dd.MM.yyyy").ToLower());
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);
            return Redirect(StartUrl + query);
        }

        /// <summary>
        /// Очищаем фильтр
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
        }

        
        [HttpPost]
        public ActionResult DeleteAnswer(Guid linkId)
        {
            var res = _cmsRepository.DeleteAnswer(linkId);
            if (res)
                return Json("Success");

            return Json("An Error Has Occourred");
        }
    }
}