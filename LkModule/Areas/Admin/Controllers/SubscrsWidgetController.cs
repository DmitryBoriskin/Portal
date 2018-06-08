using LkModule.Areas.Admin.Models;
using Portal.Areas.Admin.Controllers;
using System;
using System.Web.Mvc;
using static Portal.Areas.Admin.BeCoreController;

namespace LkModule.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("SubscrsWidget")]
    public class SubscrsWidgetController : BaseController
    {
        // GET: Admin/SubscrsWidget
        [Route]
        public ActionResult Index(Guid id)
        {
            SubscrViewModel model = new SubscrViewModel
            {
                Subscrs = _cmsRepository.GetSubscrs(id),
                SelectedSubscrs = _cmsRepository.GetSelectedSubscrs(id)
            };
            return View(model);
        }

        [Route("Add"), HttpPost]
        public ActionResult Add(Guid[] items, Guid user)
        {
            return _cmsRepository.AddUserSubscr(user, items) ? Json("Success") : Json("False");
        }

        [Route("Drop"), HttpPost]
        public ActionResult Drop(Guid item, Guid user)
        {
            return _cmsRepository.DropUserSubscr(item, user) ? Json("Success") : Json("False");
        }
    }
}