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
            var aFilter = FilterModel.Extend<LkFilter>(filter);
            StatisticsFrontModel model = new StatisticsFrontModel();
            ViewName = "~/Views/Modules/StatisticWidget/Index.cshtml";
            
                //_Repository.GetModuleView(ControllerName, ActionName);

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
                    model.AccrualsByDateJson = "[['Месяцы','рубли']," + string.Join(",", data.Where(s => s.Amount != null).Select(s => string.Format("['{0}',{1}]", s.Date.ToShortDateString(), s.Amount.Value.ToString("0.00").Replace(",", ".")))) + "]";
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