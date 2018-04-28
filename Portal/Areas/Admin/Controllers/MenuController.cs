using Portal.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    public class MenuController : CoreController
    {
        MenuViewModel model;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            model = new MenuViewModel
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
            //ViewBag.StartUrl = StartUrl;


            ViewBag.Title = "Структура CMS";
        }



        // GET: Admin/Menu
        public ActionResult Index()
        {
            model.MenuList = _cmsRepository.GetCmsMenu();
            return View(model);
        }


        //GET: Admin/Menu/item/{GUID}
        public ActionResult Item(Guid id)
        {
            model.MenuItem = _cmsRepository.GetCmsMenuItem(id);
            model.MenuGroup = _cmsRepository.GetMenuGroup();
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
    }
}