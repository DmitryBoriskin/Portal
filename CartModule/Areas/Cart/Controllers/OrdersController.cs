using CartModule.Areas.Cart.Models;
using PgDbase.entity;
using Portal.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CartModule.Areas.Cart.Controllers
{
    [Authorize]
    public class OrdersController : LayoutController
    {
        FilterModel filter;
        OrderFrontModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/Page/error/451");

            var OrderStatusDic = new Dictionary<OrderStatus, string>()
            {
                { OrderStatus.Pending, "Ожидается"},
                { OrderStatus.Processing, "Обрабатывается"},
                { OrderStatus.Shipped, "Отправлен"},
                { OrderStatus.Complete, "Выполнен"},
                { OrderStatus.Сanceled, "Аннулирован"},
                { OrderStatus.Error, "Ошибка"},
            };

           model = new OrderFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };
        }

        //Список сделанных заказов
        public ActionResult Index()
        {
            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            filter = GetFilter();
            var cFilter = FilterModel.Extend<CartFilter>(filter);
            cFilter.UserId = CurrentUser.UserId;

            if (cFilter.Date.HasValue)
                ViewBag.beginDate = cFilter.Date.Value.ToString("dd.MM.yyyy");

            if (cFilter.DateEnd.HasValue)
                ViewBag.endDate = cFilter.DateEnd.Value.ToString("dd.MM.yyyy");

            model.List = _Repository.GetOrders(cFilter);

            return View(ViewName, model);
        }

        //Инфо о сделанном заказе
        public ActionResult Item(Guid id)
        {
            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            filter = GetFilter();
            var mFilter = FilterModel.Extend<LkFilter>(filter);
            mFilter.Payed = false;

            var userId = CurrentUser.UserId;
            model.Item = _Repository.GetOrder(id);
            if (model.Item != null)
                model.Item.Products = _Repository.GetOrderedItems(id);

            return View(ViewName, model);
        }

        //Корзина
        public ActionResult New()
        {
            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            var userId = CurrentUser.UserId;

            var model = new CartFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser,
                List = _Repository.GetCartItems(userId)
            };

            return View(ViewName, model);
        }

        //Новый заказ
        public ActionResult Confirm()
        {
            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            filter = GetFilter();
            var mFilter = FilterModel.Extend<LkFilter>(filter);
            mFilter.Payed = false;

            var userId = CurrentUser.UserId;
           

            return View(ViewName, model);
        }

        //Добавление товара в корзину
        [HttpPost]
        public ActionResult OrderAddProduct(Guid productId)
        {
            var userId = CurrentUser.UserId;
            var res = _Repository.OrderAddProductToCart(userId, productId);
            if (res)
                return Json("Success");
            //return Response.Status = "OK";  //AsJson(new { status = true, reason = "OK", data = "" });
            return Json("An Error Has occourred"); //Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        //Удаление товара из корзины
        [HttpPost]
        public ActionResult OrderRemoveProduct(Guid productId)
        {
            var userId = CurrentUser.UserId;
            var res = _Repository.OrderRemoveProductFromCart(userId, productId);
            if (res)
                return Json("Success");
            //return Response.Status = "OK";  //AsJson(new { status = true, reason = "OK", data = "" });
            return Json("An Error Has occourred"); //Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        //Изменение количества товара в корзине
        [HttpPost]
        public ActionResult OrderUpdateProduct(Guid productId, int amount)
        {
            var userId = CurrentUser.UserId;
            var res = _Repository.OrderUpdateProductInCart(userId, productId, amount);
            if (res)
                return Json("Success");
            //return Response.Status = "OK";  //AsJson(new { status = true, reason = "OK", data = "" });
            return Json("An Error Has occourred"); //Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        //public ActionResult Search(string size, string page, string status, string type)
        //{
        //    string query = HttpUtility.UrlDecode(Request.Url.Query);
        //    query = AddFilterParam(query, "page", String.Empty);
        //    query = AddFilterParam(query, "size", size);
        //    query = AddFilterParam(query, "status", status);
        //    query = AddFilterParam(query, "type", type);

        //    return Redirect(StartUrl + query);
        //}

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        //public ActionResult ClearFiltr(Guid subscr)
        //{
        //    return Redirect($"{StartUrl}?subscr={subscr}");
        //}

        //[HttpPost]
        //[MultiButton(MatchFormKey = "action", MatchFormValue = "back-btn")]
        //public ActionResult Back()
        //{
        //    string par = Request.UrlReferrer.Query;
        //    string subscr = par.Replace("?subscr=", "");
        //    string url = $"/admin/subscrs/item/{subscr}";
        //    return Redirect(url);
        //}
    }
}