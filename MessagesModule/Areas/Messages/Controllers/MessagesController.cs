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
            //if (!_Repository.ModuleAllowed(ControllerName))
            //    Response.Redirect("/Page/ModuleDenied");

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
            filter = GetFilter();
            model.List = _Repository.GetHistoryMessageTheme(filter);

            return View(model);
        }
        public ActionResult Item(Guid id)
        {
            model.Theme = _Repository.GetItemMessageTheme(id);
            model.Item = new MessagesModel
            {
                Id=model.Theme.Id
            };
            return View(model);
        }
        public ActionResult Send(MessagesFrontModel backmodel)
        {
            _Repository.SendMessage(backmodel.Item);
            return Redirect("/messages/item/"+backmodel.Item.Id);
        }
        public ActionResult SendNew(MessagesFrontModel backmodel)
        {
            _Repository.CreateNewTheme(backmodel.Theme);
            return Redirect("/messages/");
        }
        
    }
}