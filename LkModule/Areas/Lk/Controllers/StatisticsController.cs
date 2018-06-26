using LkModule.Areas.Lk.Models;
using Newtonsoft.Json;
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
    public class StatisticsController : LayoutController
    {
        FilterModel filter;
        StatisticsFrontModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            //if (!_Repository.ModuleAllowed(ControllerName))
            //    Response.Redirect("/Page/ModuleDenied");

            model = new StatisticsFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };
        }

        //Статистика по платежам и начислениям
        public ActionResult Index()
        {
            filter = GetFilter();
            var aFilter = FilterModel.Extend<LkFilter>(filter);
            var pFilter = FilterModel.Extend<LkFilter>(filter);

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);

            if (userSubscr != null)
            {
               
                var accruals = _Repository.GetAccruals(userSubscr.Id, aFilter);
                if (accruals.Items != null)
                {
                    var data = accruals.Items.Reverse();
                    filter.Date = data.First().Date;
                    filter.DateEnd = DateTime.Now;
                    model.AccrualsByDateJson = "[['Месяцы','рубли']," + string.Join(",", data.Where(s => s.Amount != null).Select(s => string.Format("['{0}',{1}]", s.Date.ToShortDateString(), s.Amount.Value.ToString("0.00").Replace(",",".")))) + "]";
                }
                //model.Payments = _Repository.GetPayments(userSubscr.Id, filter);
                //model.Consumption = _Repository.GetMeters(device)

                // model.AccrualsByDateJson = "[['Месяцы','рубли'],['декабрь',8651486.31],['январь',13223292.59],['февраль',11916139.67],['март',1501363.17],['апрель',10639269.23],['май',11251567.51],['июнь',0.00]]";

              
                var payments = _Repository.GetPayments(userSubscr.Id, pFilter);
                if (payments.Items != null)
                {
                    var data = payments.Items.Reverse();
                    model.PaymentsByDateJson = "[['Месяцы','рубли']," + string.Join(",", data.Where(s => s.Amount != null).Select(s => string.Format("['{0}',{1}]", s.Date.ToShortDateString(), s.Amount.Value.ToString("0.00").Replace(",", ".")))) + "]";
                }

                //var cFilter = FilterModel.Extend<LkFilter>(filter);
                //var consumption = _Repository.GetMeters(userSubscr.Id, cFilter);
                //if (consumption.Items != null)
                //{
                //    var data = payments.Items.Reverse();
                //    model.PaymentsByDateJson = "[['Месяцы','рубли']," + string.Join(",", data.Where(s => s.Amount != null).Select(s => string.Format("['{0}',{1}]", s.Date.ToShortDateString(), s.Amount.Value.ToString("0.00").Replace(",", ".")))) + "]";
                //}

                //model.PaymentsByDateJson = "[['Месяцы','рубли'],['декабрь',8651486.31],['январь',13223292.59],['февраль',11916139.67],['март',1501363.17],['апрель',10639269.23],['май',11251567.51],['июнь',0.00]]";
                //model.ConsumptionByDateJson = "[['Месяцы','Потребление'],['декабрь',8651486.31],['январь',13223292.59],['февраль',11916139.67],['март',1501363.17],['апрель',10639269.23],['май',11251567.51],['июнь',0.00]]";
            }

            if (filter.Date.HasValue)
                ViewBag.beginDate = filter.Date.Value.ToString("dd.MM.yyyy");

            if (filter.DateEnd.HasValue)
                ViewBag.endDate = filter.DateEnd.Value.ToString("dd.MM.yyyy");
           

            return View(model);
        }
    }
}