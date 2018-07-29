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
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");
        }

        // GET: MsgWidget/Index
        public ActionResult Index()
        {
            MsgWidgetFrontModel model = new MsgWidgetFrontModel();
            model.MsgList = _Repository.GetNewMessageTheme();

            if (model.MsgList != null)
            {
                int n = model.MsgList.Count();
                model.NewMsgCount = n;

                model.NewMsgCountText = "У вас ";
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
                        model.NewMsgCountText += n + " новых сообщений";
                        break;
                }
            }

            return PartialView(ViewName, model);
        }

        public ActionResult LastMessages()
        {
            var model = new MsgWidgetFrontModel
            {
                MsgList = _Repository.GetLastInboxMessage()
            };
            return PartialView(ViewName, model);
        }
    }
}