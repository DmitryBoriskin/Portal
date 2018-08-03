using LkModule.Areas.Lk.Models;
using PgDbase.entity;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LkModule.Areas.Lk.Controllers
{
    public class StatisticWidgetController : CoreController
    {
        // GET: Lk/StatisticWidget
        public ActionResult Index()
        {
            FilterModel filter;
            filter = GetFilter();
            var pFilter = FilterModel.Extend<LkFilter>(filter);
            pFilter.Date = DateTime.Parse("01-01-" + DateTime.Now.Year);
            pFilter.DateEnd = DateTime.Now;

            StatisticsFrontModel model = new StatisticsFrontModel();
            //ViewName = "~/sites/rushydro/view/module/lk/StatisticWidget.cshtml";
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if(ViewName!=null) ViewName = "~/sites/rushydro/view/module/lk/StatisticWidget.cshtml";

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);


            if (userSubscr != null)
            {
                var payments = _Repository.GetPayments(userSubscr.Id, pFilter);
                if (payments.Items != null && payments.Items.Count() > 0)
                {
                    var data = payments.Items
                        .GroupBy(p => p.Period)
                        .Select(p => new PaymentModel()
                            {
                                Date = DateTime.Parse( $"{ p.First().Date.Year }-{p.First().Date.Month}-01"),
                                //Quantity = p.Count(),
                                Period = p.First().Period,
                                Amount = p.Sum(c => c.Amount),
                            }).ToArray()
                    .Reverse();

                    filter.Date = data.First().Date;
                    filter.DateEnd = DateTime.Now;
                    model.AccrualsByDateJson = "[['Месяц','руб']," + string.Join(",", data.Where(s => s.Amount != null).Select(s => string.Format("['{0}',{1}]", s.Date.ToString("MMM"), s.Amount.Value.ToString("0.00").Replace(",", ".")))) + "]";
                }
            }


            if (filter.Date.HasValue)
                ViewBag.beginDate = filter.Date.Value.ToString("dd.MM.yyyy");

            if (filter.DateEnd.HasValue)
                ViewBag.endDate = filter.DateEnd.Value.ToString("dd.MM.yyyy");

            return PartialView(ViewName, model);
        }
    }
}