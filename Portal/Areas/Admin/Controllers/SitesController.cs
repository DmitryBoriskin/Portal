using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin
{
    public class SitesController : CoreController
    {
        SitesViewModel model;
        FilterModel filter;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new SitesViewModel
            {
                DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };
            if (AccountInfo != null)
            {
                model.Menu = _cmsRepository.GetCmsMenu(AccountInfo.Id);

            }
        }


        // GET: Admin/Sites
        public ActionResult Index()
        {
            PgDbase.entity.FilterModel filter = GetFilter();
            model.List = _cmsRepository.GetSitesList(filter);

            return View(model);
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

        //GET: Admin/Sites/item/{GUID}
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetSites(id);
            return View("Item", model);
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
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, SitesViewModel backModel)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };

            if (ModelState.IsValid)
            {
                backModel.Item.Id = id;
                if (_cmsRepository.CheckSiteExist(id))
                {
                    _cmsRepository.UpdateSite(backModel.Item);
                    message.Info = "Запись обновлена";
                }
                else
                {
                    _cmsRepository.InsertSites(backModel.Item);
                    message.Info = "Запись добавлена";
                }
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = StartUrl + Request.Url.Query, Text = "вернуться в список" },
                    new ErrorMessageBtnModel { Url = StartUrl+"item/"+id, Text = "ок", Action = "false" }
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
        public ActionResult Delete(Guid id)
        {
            _cmsRepository.DeleteSite(id);
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


        /// <summary>
        /// Добавление домена
        /// </summary>
        /// <returns>перезагружает страницу</returns>
        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "add-new-domain")]
        public ActionResult AddDomain()
        {
            try
            {
                Guid id = Guid.Parse(Request["Item.Id"]);                
                string Domain = Request["new_domain"].Replace(" ", "");

                _cmsRepository.InsertDomain(Domain, id);
            }
            catch (Exception ex)
            {
                throw new Exception("SitesController > AddDomain: " + ex);
            }
            return Redirect(((System.Web.HttpRequestWrapper)Request).RawUrl);
        }


        [HttpPost]
        public ActionResult SetDomainDefault(Guid id)
        {
            var res = _cmsRepository.SetDomainDefault(id);
            if (res)
                return Json("Success");

            return Json("An Error Has occourred");
        }

        [HttpPost]
        public ActionResult DelDomain(Guid id)
        {
            if(_cmsRepository.DeleteDomain(id)) return null;
            return Json("default");

        }


    }
}