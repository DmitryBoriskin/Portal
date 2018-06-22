using PgDbase.entity;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessagesModule.Areas.Messages.Controllers
{
    [Authorize]
    public class MsgController : CoreController
    {
        // GET: Msg/Msg
        public ActionResult Index()
        {            
            List<MessagesModel> NewMsgList = new List<MessagesModel>();
            
            return View();
        }
    }
}