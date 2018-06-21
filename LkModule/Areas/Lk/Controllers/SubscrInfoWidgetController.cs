using PgDbase.entity;
using Portal.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LkModule.Areas.Lk.Controllers
{
    [Authorize]
    public class SubscrInfoWidgetController : CoreController
    {
       
        public ActionResult Info()
        {
            var model = new List<SubscrShortModel>();

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);

            if (userSubscr != null)
            {
                model = _Repository.GetSubscrInfoForTopPannel(userId);
            }

            return View(model);
        }

    }
}