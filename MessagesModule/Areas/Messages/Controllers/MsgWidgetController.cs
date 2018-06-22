using MessagesModule.Areas.Messages.Models;
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
    public class MsgWidgetController : CoreController
    {
        // GET: MsgWidget/Index
        public ActionResult Index()
        {
            MsgWidgetFrontModel model = new MsgWidgetFrontModel();
            model.MsgList = _Repository.GetNewMessageTheme();

            model.NewMsgCountText = "У вас ";
            int n= model.MsgList.Count();
            model.NewMsgCount = n;            
            switch (n)
            {
                case (0):
                    model.NewMsgCountText += "нет новых сообщений";
                    break;
                case (1):
                    model.NewMsgCountText += "1 новое сообщение";
                    break;
                case (2):
                    model.NewMsgCountText += "2 новых сообщения";
                    break;
                default:
                    model.NewMsgCountText += n+ " новых сообщений";
                    break;
            }

            return PartialView(model);
        }
    }
}