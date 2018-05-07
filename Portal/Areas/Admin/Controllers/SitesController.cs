using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class SitesController : CoreController
    {
        SitesViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new SitesViewModel()
            {
                PageName = "Все сайты",
                DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };
            if (AccountInfo != null)
            {
                model.Menu = MenuCmsCore;
                model.MenuModul = MenuModulCore;
            }
        }


        // GET: Admin/Sites
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List = _cmsRepository.GetSites(filter);

            return View(model);
        }

        //GET: Admin/Sites/item/{GUID}
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetSite(id);

            if (model.Item != null)
            {
                model.Item.Modules = _cmsRepository.GetSiteModulesList(id);
                model.Modules = _cmsRepository.GetModulesList();
            }
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

        #region Домены
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
        public ActionResult SetSiteDomainDefault(Guid id)
        {
            var res = _cmsRepository.SetDomainDefault(id);
            if (res)
                return Json("Success");

            return Json("An Error Has Occourred");
        }

        [HttpPost]
        public ActionResult DeleteSiteDomain(Guid id)
        {
            var res = _cmsRepository.DeleteDomain(id);
            if (res)
                return Json("Success");

            return Json("An Error Has Occourred");

        }

        #endregion

        #region Модули

        //GET: Admin/Sites/Module/{GUID}
        public ActionResult Module(Guid id)
        {
            //filter = GetFilter();
            var module = _cmsRepository.GetSiteModule(id);
            model.Item = _cmsRepository.GetSite(module.SiteId);

            if (model.Item != null)
                model.Item.Modules = new SiteModuleModel[] { module };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddSiteModule(Guid siteId, Guid moduleId)
        {
            var res = _cmsRepository.InsertSiteModuleLink(siteId, moduleId);
            if (res)
                return Json("Success");

            return Json("An Error Has Occourred");
        }

        [HttpPost]
        public ActionResult DeleteSiteModule(Guid linkId)
        {
            var res = _cmsRepository.DeleteSiteModuleLink(linkId);
            if (res)
                return Json("Success");

            return Json("An Error Has Occourred");
        }
        #endregion
    }
}