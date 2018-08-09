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
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

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
               
#pragma warning disable CS1061 // 'FrontRepository' does not contain a definition for 'GetInvoices' and no extension method 'GetInvoices' accepting a first argument of type 'FrontRepository' could be found (are you missing a using directive or an assembly reference?)
                var accruals = _Repository.GetInvoices(userSubscr.Id, aFilter);
#pragma warning restore CS1061 // 'FrontRepository' does not contain a definition for 'GetInvoices' and no extension method 'GetInvoices' accepting a first argument of type 'FrontRepository' could be found (are you missing a using directive or an assembly reference?)
                if (accruals.Items != null && accruals.Items.Count() > 0)
                {
                    var data = accruals.Items
                       .GroupBy(p => p.Period)
                       .Select(p => new PaymentModel()
                       {
                           Date = DateTime.Parse($"{ p.First().Date.Year }-{p.First().Date.Month}-01"),
                            //Quantity = p.Count(),
                           Period = p.First().Period,
                           Amount = p.Sum(c => c.Amount),
                       }).ToArray()
                   .Reverse();

                    filter.Date = data.First().Date;
                    filter.DateEnd = DateTime.Now;
                    model.InvoicesByDateJson = "[['Месяцы','рубли']," + string.Join(",", data.Where(s => s.Amount != null).Select(s => string.Format("['{0}',{1}]", s.Date.ToString("MM.yyyy"), s.Amount.Value.ToString("0.00").Replace(",",".")))) + "]";
                }
                //model.Payments = _Repository.GetPayments(userSubscr.Id, filter);
                //model.Consumption = _Repository.GetMeters(device)

                // model.InvoicesByDateJson = "[['Месяцы','рубли'],['декабрь',8651486.31],['январь',13223292.59],['февраль',11916139.67],['март',1501363.17],['апрель',10639269.23],['май',11251567.51],['июнь',0.00]]";


                var payments = _Repository.GetPayments(userSubscr.Id, pFilter);
                if (payments.Items != null && payments.Items.Count() > 0)
                {
                    var data = payments.Items
                       .GroupBy(p => p.Period)
                       .Select(p => new PaymentModel()
                       {
                           Date = DateTime.Parse($"{ p.First().Date.Year }-{p.First().Date.Month}-01"),
                           //Quantity = p.Count(),
                           Period = p.First().Period,
                           Amount = p.Sum(c => c.Amount),
                       }).ToArray();

                    model.PaymentsByDateJson = "[['Месяцы','рубли']," + string.Join(",", data.Where(s => s.Amount != null).Select(s => string.Format("['{0}',{1}]", s.Date.ToString("MM.yyyy"), s.Amount.Value.ToString("0.00").Replace(",", ".")))) + "]";
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
           

            return View(ViewName, model);
        }
    }
}