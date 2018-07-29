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
    public class MessagesController : LayoutController
    {
        FilterModel filter;
        MessagesFrontModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");


            model = new MessagesFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };
        }

        // GET: Msg/Messages
        public ActionResult Index()
        {
            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            filter = GetFilter();
            model.List = _Repository.GetHistoryMessageTheme(filter);

            return View(ViewName,model);
        }

        public ActionResult Item(Guid id)
        {
            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            model.Theme = _Repository.GetItemMessageTheme(id);
            model.Item = new MessagesModel
            {
                Id = model.Theme.Id
            };
            return View(ViewName, model);
        }

        public ActionResult Send(MessagesFrontModel backmodel)
        {
            _Repository.SendMessage(backmodel.Item);
            return Redirect("/messages/item/" + backmodel.Item.Id);
        }

        public ActionResult SendNew(MessagesFrontModel backmodel)
        {
            _Repository.CreateNewTheme(backmodel.Theme);
            return Redirect("/messages/");
        }

    }
}