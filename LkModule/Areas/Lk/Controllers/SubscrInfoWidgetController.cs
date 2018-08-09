using LkModule.Areas.Lk.Models;
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
        Guid SubscrId;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var userId = CurrentUser.UserId;
            SubscrId=_Repository.GetUserSubscrDefault(userId).Id;
        }

        /// <summary>
        /// текущий ЛС и его баланс
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            var model = new SubscrWidgetFrontModel();

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);

            if (userSubscr != null)
            {
                model.List = _Repository.GetSubscrSaldoInfo(userId);
                model.Item = (model.List != null) ? model.List.SingleOrDefault(s => s.Default == true) : null;

            }

            return PartialView(ViewName, model);
        }
        /// <summary>
        /// все подключенные лицевые счета
        /// </summary>
        /// <returns></returns>
        public ActionResult Info()
        {
            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            var model = new SubscrWidgetFrontModel();

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);

            if (userSubscr != null)
            {
                model.List = _Repository.GetSubscrSaldoInfo(userId);
                model.Item = (model.List != null) ? model.List.SingleOrDefault(s => s.Default == true): null;

            }

            return PartialView(ViewName, model);
        }

        public ActionResult Balance()
        {
            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            var model = new SubscrWidgetFrontModel();

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);

            if (userSubscr != null)
            {
                model.List = _Repository.GetSubscrSaldoInfo(userId);
                model.Item = (model.List != null) ? model.List.SingleOrDefault(s => s.Default == true) : null;

            }

            return PartialView(ViewName, model);
        }

        [HttpPost]
        public ActionResult SetUserSubscrDefault(Guid subscrId)
        {
            var userId = CurrentUser.UserId;

            var res = _Repository.SetUserSubscrDefault(subscrId, userId);
            if (res)
                return Json("success");

            return Json("An Error Has Occourred");
        }



        public ActionResult Manager()
        {
            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            SubscrManager model = new SubscrManager();
            model = _Repository.GetManager(SubscrId);

            return PartialView(ViewName, model);//, model
        }

        //public ActionResult AccountsAndPayments()
        //{
        //    //Есть ли у сайта доступ к модулю
        //    if (!_Repository.ModuleAllowed(ControllerName))
        //        Response.Redirect("/page/error/451");

        //    //Шаблон
        //    ViewName = _Repository.GetModuleView(ControllerName, ActionName);
        //    return PartialView(ViewName, model);
        //}
    }
}