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
    public class PuController : LayoutController
    {
        FilterModel filter;
        PuFrontModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/Page/error/451");

            model = new PuFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            filter = GetFilter();

            if (!filter.Date.HasValue)
                filter.Date = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
            if (!filter.DateEnd.HasValue)
                filter.DateEnd = DateTime.Now;

            var userId = CurrentUser.UserId;
            var userSubscr = _Repository.GetUserSubscrDefault(userId);
            if (userSubscr != null)
            {
                var pFilter = FilterModel.Extend<LkFilter>(filter);
                model.List = _Repository.GetSubscrDevices(userSubscr.Id, pFilter);

                var cons = _Repository.GetConsumptionStatistics(userSubscr.Id, pFilter);
                if (cons != null && cons.Count() > 0)
                {
                    var data = cons.Reverse();
                    model.СonsumptionDataJson = "[['Месяц','Потребление', 'Мощность']," + string.Join(",", data.Select(s => string.Format("['{0}',{1},{2}]", s.Period.Value.ToString("MMM"), s.ConsumptionAmount1.Value.ToString("0.00").Replace(",", "."), s.ConsumptionAmount2.Value.ToString("0.00").Replace(",", ".")))) + "]";
                }
            }
            model.Filter = filter;

            return View(ViewName, model);
        }

        /// <summary>
        /// Список Пу
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Devices(Guid id)
        {
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            model.Devices = _Repository.GetPuModel(id);
            filter = GetFilter();
            model.DevicesMeter = _Repository.GetMeterModel(id, filter);

            return View(ViewName, model);
        }

        
        [HttpPost]
        public ActionResult GetPuMeters(Guid device)
        {
            //ViewName = "/Views/Modules/Pu/Part/CounterReading.cshtml";
            ViewName = "/sites/rushydro/view/module/lk/PU/Part/CounterReading.cshtml";
            var model = _Repository.GetMeters(device);

            //var json = JsonConvert.SerializeObject(meters);
            //return Json(json);

            return PartialView(ViewName, model);
        }

    }
}