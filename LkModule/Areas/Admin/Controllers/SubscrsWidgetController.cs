using LkModule.Areas.Admin.Models;
using Portal.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Controllers
{
    public class SubscrsWidgetController : BaseController
    {

        // GET: Admin/SubscrsWidget
        [Route]
        public ActionResult Index()
        {
            // привязываем лс к пользователю

            //if (backModel.Item.Subscrs != null && backModel.Item.Subscrs.Count() > 0)
            //{
            //    _cmsRepository.UpdateUserSubscrs(id, backModel.Item.Subscrs);
            //}

            SubscrViewModel model = new SubscrViewModel
            {
                Subscrs = _cmsRepository.GetSubscrs()
            };
            return View(model);
        }
    }
}