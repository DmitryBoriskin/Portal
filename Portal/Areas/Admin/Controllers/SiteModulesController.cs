using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class SiteModulesController : BeCoreController
    {
        //public SiteModulesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //  : base(userManager, signInManager) { }

        SitesViewModel model;
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new SitesViewModel()
            {
                PageName = PageName,
                DomainName = Domain,
                Account = AccountInfo,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName
            };
            if (AccountInfo != null)
                model.Menu = _cmsRepository.GetCmsMenu();
        }

        // GET: Admin/SiteModules
        public ActionResult Index()
        {
            filter = GetFilter();
            var mfilter = FilterModel.Extend<ModuleFilter>(filter);
            model.Item = _cmsRepository.GetSite(SiteId);
            model.Item.Modules = _cmsRepository.GetSiteModulesList(SiteId);

            ViewBag.SearchText = filter.SearchText;
            return View(model);
        }


        // GET: Admin/SiteModules/item/{Guid:id}
        public ActionResult Item(Guid id)
        {
            //filter = GetFilter();
            var module = _cmsRepository.GetSiteModule(id);
            model.Item = _cmsRepository.GetSite(module.SiteId);

            if (model.Item != null)
            {
                var templates = _cmsRepository.GetTemplatesList();
                if (templates != null)
                    module.Templates = templates
                            .Where(t => t.Controller.Id == module.ModuleId)
                            .ToArray();

                model.Item.Modules = new SiteModuleModel[] { module };
            }

            return View("Item", model);
        }


        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, string group, string size)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "searchtext", searchtext);
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
        public ActionResult SetDefaultTemplate(Guid linkId, Guid templateId)
        {
            var res = _cmsRepository.SetSiteModuleTemplateDefault(linkId, templateId);
            if (res)
                return Json("Success");

            return Json("An Error Has Occourred");
        }
    }
}