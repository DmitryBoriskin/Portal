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
    public class PaymentsController : LayoutController
    {
        FilterModel filter;
        PaymentFrontModel model;

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

            model = new PaymentFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };
        }

        // GET: Admin/Payments
        public ActionResult Index()
        {
            filter = GetFilter();

            if (!filter.Date.HasValue)
                filter.Date = DateTime.Now.AddMonths(-1);
            if (!filter.DateEnd.HasValue)
                filter.DateEnd = DateTime.Now;

            model.Filter = filter;
            var mFilter = FilterModel.Extend<LkFilter>(filter);

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);

            if (userSubscr != null)
            {
                model.List = _Repository.GetPayments(userSubscr.Id, mFilter);
                if (model.List != null && model.List.Items != null && model.List.Items.Count() > 0)
                {
                    var summaZaPeriod = model.List.Items.Select(s => s.Amount).Sum();
                    model.SummaZaPeriod = summaZaPeriod.HasValue ? summaZaPeriod.Value : 0m;
                }
            }

            return View(ViewName, model);
        }

    }
}