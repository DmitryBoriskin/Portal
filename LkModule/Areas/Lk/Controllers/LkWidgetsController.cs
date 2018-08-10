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
    public class LkWidgetsController : CoreController
    {
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
                model.Item = (model.List != null) ? model.List.SingleOrDefault(s => s.Default == true) : null;
            }

            return PartialView(ViewName, model);
        }

        /// <summary>
        /// блок баланса
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Менеджер
        /// </summary>
        /// <returns></returns>
        public ActionResult Manager()
        {
            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);

            SubscrManager model = new SubscrManager();
            model = _Repository.GetManager(userSubscr.Id);

            return PartialView(ViewName, model);//, model
        }

        /// <summary>
        /// Графики - ключевые показатели
        /// </summary>
        /// <returns></returns>
        public ActionResult Chart()
        {
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            var filter = GetFilter();

            if (!filter.Date.HasValue)
                filter.Date = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
            if (!filter.DateEnd.HasValue)
                filter.DateEnd = DateTime.Now;

            var model = new StatisticsFrontModel();
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);

            model.Filter = filter;

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);
            if (userSubscr != null)
            {
                var pFilter = FilterModel.Extend<LkFilter>(filter);

                //Для двойного графика (начисления, платежи)
                var balances = _Repository.GetDebitCreditList(userSubscr.Id, pFilter);

                if (balances != null && balances.Count() > 0)
                {
                    //foreach (var balance in balances)
                    //{
                    //    if (balance.PeriodId.HasValue)
                    //    {
                    //        var str = balance.PeriodId.Value.ToString();
                    //        var year = str.Substring(0, 4);
                    //        var month = str.Substring(4, 1) == "0" ? str.Substring(5, 1) : str.Substring(4, 2);

                    //        balance.Period = new DateTime(int.Parse(year), int.Parse(month), 1, 0, 0, 0);
                    //    }
                    //}
                    var data = balances.Reverse();
                    model.InvoicesAndPaymentsByDateJson = "[['Месяц','Начисления','Платежи']," + string.Join(",", data.Select(s => string.Format("['{0}',{1}, {2}]", s.Period.Value.ToString("MMM"), s.InvoiceAmount.Value.ToString("0.00").Replace(",", "."), s.PaymentAmount.Value.ToString("0.00").Replace(",", ".")))) + "]";
                }

                #region Графики по отдельности (закоментировано)
                //Для графика (начисления)
                //var invoices = _Repository.GetInvoicesList(userSubscr.Id, pFilter);
                //if(invoices!= null && invoices.Count()>0)
                //{
                //    var data = invoices.Where(i => i.DocTypeId == 39)
                //        .GroupBy(p => p.Period)
                //        .Select(p => new InvoiceModel()
                //        {
                //            Date = new DateTime(p.First().Date.Year, p.First().Date.Month, 1, 0, 0, 0, 0),
                //            Period = p.First().Period,
                //            Amount = p.Sum(c => c.Amount),
                //        }).ToArray();

                //    model.PaymentsByDateJson = "[['Месяц','руб']," + string.Join(",", data.Where(s => s.Amount != null).Select(s => string.Format("['{0}',{1}]", s.Date.ToString("MMM"), s.Amount.Value.ToString("0.00").Replace(",", ".")))) + "]";
                //}


                //Для графика (платежи)
                //var payments = _Repository.GetPaymentsList(userSubscr.Id, pFilter);
                //if (payments != null && payments.Count() > 0)
                //{
                //    var data = payments
                //        .GroupBy(p => p.Period)
                //        .Select(p => new PaymentModel()
                //        {
                //            Date = new DateTime(p.First().Date.Year, p.First().Date.Month, 1, 0, 0, 0, 0),
                //            Period = p.First().Period,
                //            Amount = p.Sum(c => c.Amount),
                //        }).ToArray();

                //    model.InvoicesByDateJson = "[['Месяц','руб']," + string.Join(",", data.Where(s => s.Amount != null).Select(s => string.Format("['{0}',{1}]", s.Date.ToString("MMM"), s.Amount.Value.ToString("0.00").Replace(",", ".")))) + "]";
                //}

                #endregion
            }

            return PartialView(ViewName, model);
        }

        /// <summary>
        /// Дебит кредит баланс
        /// </summary>
        /// <returns></returns>
        public ActionResult DebitCredit()
        {
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/page/error/451");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            var filter = GetFilter();

            //if (!filter.Date.HasValue)
            //    filter.Date = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
            //if (!filter.DateEnd.HasValue)
            //    filter.DateEnd = DateTime.Now;

            var model = new StatisticsFrontModel();
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);

            model.Filter = filter;

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);

            if (userSubscr != null)
            {
                var pFilter = FilterModel.Extend<LkFilter>(filter);
                pFilter.Size = 10;


                var subscrBalances = _Repository.GetSubscrSaldoInfo(userId);
                model.Balance = (subscrBalances != null) ? subscrBalances.SingleOrDefault(s => s.Default == true) : null;

                model.DebitCreditData = _Repository.GetDebitCreditData(userSubscr.Id, pFilter);
                //if (model.DebitCreditData != null && model.DebitCreditData.Items != null && model.DebitCreditData.Items.Count() > 0)
                //{
                //    foreach (var balance in model.DebitCreditData.Items)
                //    {
                //        if (balance.PeriodId.HasValue)
                //        {
                //            var str = balance.PeriodId.Value.ToString();
                //            var year = str.Substring(0, 4);
                //            var month = str.Substring(4, 1) == "0" ? str.Substring(5, 1) : str.Substring(4, 2);

                //            balance.Period = new DateTime(int.Parse(year), int.Parse(month), 1, 0, 0, 0);
                //        }
                //    }
                //}
            }

            return PartialView(ViewName, model);

        }
    }
}