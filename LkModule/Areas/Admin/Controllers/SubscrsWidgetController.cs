using LkModule.Areas.Admin.Models;
using Portal.Areas.Admin.Controllers;
using System;
using System.Web.Mvc;

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
                Subscrs = _cmsRepository.GetSubscrs(),
                SelectedSubscrs = _cmsRepository.GetSelectedSubscrs(id)
            };
            return View(model);
        }

        [Route("Update"), HttpPost]
        public ActionResult Update(Guid[] items, Guid user)
        {
            _cmsRepository.UpdateUserSubscrs(user, items);
            return null;
        }
    }
}