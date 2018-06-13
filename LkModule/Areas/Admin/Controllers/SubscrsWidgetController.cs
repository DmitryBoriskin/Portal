using LkModule.Areas.Admin.Models;
using Portal.Areas.Admin.Controllers;
using System;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Controllers
{
    public class SubscrsWidgetController : BaseController
    {
        // GET: Admin/SubscrsWidget
        public ActionResult Index(Guid id)
        {
            SubscrViewModel model = new SubscrViewModel
            {
                Subscrs = _cmsRepository.GetSubscrs(id),
                SelectedSubscrs = _cmsRepository.GetSelectedSubscrs(id)
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Add(Guid[] items, Guid user)
        {
            return _cmsRepository.AddUserSubscr(user, items) ? Json("Success") : Json("False");
        }

        [HttpPost]
        public ActionResult Drop(Guid item, Guid user)
        {
            return _cmsRepository.DropUserSubscr(item, user) ? Json("Success") : Json("False");
        }
    }
}