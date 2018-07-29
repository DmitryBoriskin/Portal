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
    public class ProductsController : LayoutController
    {
        FilterModel filter;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/Page/error/451");
        }

        //Список категорий продуктов
        public ActionResult Index()
        {
            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            filter = GetFilter();
            var cFilter = FilterModel.Extend<CartFilter>(filter);

            var model = new ProductCategoryFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };

            model.List = _Repository.GetProductCategories(cFilter);

            return View(ViewName, model);
        }

        //Список продуктов категории
        public ActionResult List(Guid id)
        {
            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            var model = new ProductCategoryFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };

            model.Item = _Repository.GetProductCategory(id);
            if (model.Item != null)
            {
                filter = GetFilter();
                var cFilter = FilterModel.Extend<CartFilter>(filter);
                cFilter.CategoryId = id;

                model.Item.Products = _Repository.GetProducts(cFilter);

                var userId = CurrentUser.UserId;
                model.InCart = _Repository.GetProductsInCart(userId);
            }

            return View(ViewName, model);
        }

        //Описание продукта
        public ActionResult Item(Guid id)
        {
            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");

            var model = new ProductFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };

            var userId = CurrentUser.UserId;

            model.Item = _Repository.GetProduct(id);
            model.InCart = _Repository.GetProductsInCart(userId);

            return View(ViewName, model);
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