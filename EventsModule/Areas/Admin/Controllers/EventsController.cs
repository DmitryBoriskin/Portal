using EventsModule.Areas.Admin.Models;
using PgDbase.entity;
using Portal.Areas.Admin;
using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventsModule.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("Events")]
    public class EventsController : BeCoreController
    {
        EventViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new EventViewModel
            {
                PageName = PageName,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                Menu = MenuCmsCore,
                MenuModules = MenuModulCore
            };
        }

        // GET: Admin/Events
        [Route]
        public ActionResult Index()
        {
            #region filter
            filter = GetFilter();
            var mfilter = FilterModel.Extend<EventFilterModel>(filter);

            var annual = false;
            if (!String.IsNullOrEmpty(Request.QueryString["annual"]))
            {
                bool.TryParse(Request.QueryString["annual"], out annual);
                mfilter.Annual = annual;
            } 
            #endregion
            model.List=_cmsRepository.GetEventsList(mfilter);
            return View(model);
        }        
        [Route, HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);
            return Redirect(StartUrl + "item/" + Guid.NewGuid() + "/" + query);
        }


        [Route, HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, bool enabled, bool annual, string size, DateTime? datestart, DateTime? dateend)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "searchtext", searchtext);            
            query = AddFilterParam(query, "disabled", (!enabled).ToString().ToLower());
            query = AddFilterParam(query, "annual", (!annual).ToString().ToLower());
            if (datestart.HasValue)
                query = AddFilterParam(query, "datestart", datestart.Value.ToString("dd.MM.yyyy").ToLower());
            if (dateend.HasValue)
                query = AddFilterParam(query, "dateend", dateend.Value.ToString("dd.MM.yyyy").ToLower());
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);

            return Redirect(StartUrl + query);
        }


  
        [Route("item/{id:guid}"), HttpGet]
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetEventItem(id);
            var Date = DateTime.Today;
            if (model.Item != null)
            {
                ViewBag.Photo = model.Item.Photo;
                Date = model.Item.Date;
            }
            ViewBag.DataPath = Settings.UserFiles + SiteDir + Settings.MaterialsDir + Date.ToString("yyyy_MM") + "/" + Date.ToString("dd") + "/" + id + "/";
            return View("Item", model);
        }
        [Route("item/{id:guid}"), HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, EventViewModel backModel, HttpPostedFileBase upload)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            backModel.Item.Guid = id;
            if (ModelState.IsValid)
            {
                #region Изображение
                if (upload != null)
                {
                    #region добавление изображения
                    string Path = Settings.UserFiles + SiteDir + Settings.EventsDir + backModel.Item.Date.ToString("yyyy_MM") + "/" + backModel.Item.Date.ToString("dd") + "/" + id + "/";

                    #region оригинал
                    string PathOriginal = Path + "original/";
                    if (!Directory.Exists(PathOriginal)) { Directory.CreateDirectory(Server.MapPath(PathOriginal)); }
                    upload.SaveAs(Server.MapPath(Path + "original/" + upload.FileName));
                    #endregion

                    if (upload != null && upload.ContentLength > 0)
                        try
                        {
                            backModel.Item.Photo = Files.SaveImageResize(upload, Path, 540, 360);
                        }
                        catch (Exception ex)
                        {
                            ViewBag.Message = "Произошла ошибка: " + ex.Message.ToString();
                        }
                    else
                    {
                        ViewBag.Message = "Вы не выбрали файл.";
                    }
                    #endregion
                }
                #endregion


                if (_cmsRepository.CheckEvets(id))
                {
                    message.Info = "Запись обновлена";
                    _cmsRepository.UpdateEvent(backModel.Item);
                }
                else
                {
                    _cmsRepository.InsertEvent(backModel.Item);
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
                #region error no valid
                message.Info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом";
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = "#", Text = "ок", Action = "false" }
                }; 
                #endregion
            }

            var DataDir = DateTime.Today;

            backModel.Item.Guid = id;
            ViewBag.Photo = backModel.Item.Photo;
            ViewBag.DataPath = Settings.UserFiles + SiteDir + Settings.EventsDir + DataDir.ToString("yyyy_MM") + "/" + DataDir.ToString("dd") + "/" + id + "/";

            model.ErrorInfo = message;
            return View("item", model);
        }

        [Route("item/{id:guid}"), HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            _cmsRepository.DeleteEvent(id);
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

        [Route("item/{id:guid}"), HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect(StartUrl + Request.Url.Query);
        }
        [Route, HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }

        [Route("item/{id:guid}"), HttpGet]
        public ActionResult Widget()
        {
            return View();
        }
    }
}