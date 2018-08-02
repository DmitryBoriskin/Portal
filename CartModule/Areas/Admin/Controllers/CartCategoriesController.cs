using CartModule.Areas.Admin.Models;
using PgDbase.entity;
using Portal.Areas.Admin;
using Portal.Areas.Admin.Controllers;
using Portal.Areas.Admin.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CartModule.Areas.Admin.Controllers
{
    public class CartCategoriesController : BeCoreController
    {
        FilterModel filter;
        CartCategoryViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_cmsRepository.ModuleAllowed(ControllerName))
                Response.Redirect("/Admin/");

            model = new CartCategoryViewModel()
            {
                SiteId = SiteId,
                PageName = PageName,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                MenuCMS = MenuCmsCore,
                MenuModules = MenuModulCore
            };
        }


        public ActionResult Index()
        {
            model.PageName = "Категории товаров и услуг";
            filter = GetFilter();
            model.Filter = filter;
            var cFilter = FilterModel.Extend<CartFilter>(filter);
            model.List = _cmsRepository.GetCartCategories(cFilter);

            return View(model);
        }

        public ActionResult Item(Guid id)
        {
            model.PageName = "Категория товаров и услуг";
            model.Item = _cmsRepository.GetCartCategory(id);
            if (model.Item != null)
            {
                filter = GetFilter();
                var cFilter = FilterModel.Extend<CartFilter>(filter);
                cFilter.CategoryId = id;

                model.ProductsList = _cmsRepository.GetProductsList(cFilter);
                ViewBag.Image = model.Item.Icon;
            }

            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "insert-btn")]
        public ActionResult Insert()
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "page", String.Empty);

            return Redirect($"{StartUrl}item/{Guid.NewGuid()}/{query}");
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "search-btn")]
        public ActionResult Search(string searchtext, bool enabled, string size)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "searchtext", searchtext);
            query = AddFilterParam(query, "disabled", (!enabled).ToString().ToLower());
            query = AddFilterParam(query, "page", String.Empty);
            query = AddFilterParam(query, "size", size);

            return Redirect(StartUrl + query);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "clear-btn")]
        public ActionResult ClearFiltr()
        {
            return Redirect(StartUrl);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "cancel-btn")]
        public ActionResult Cancel()
        {
            return Redirect($"{StartUrl}{Request.Url.Query}");
        }


        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, CartCategoryViewModel backModel, HttpPostedFileBase upload)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            if (ModelState.IsValid)
            {
                backModel.Item.Id = id;
                
                #region Сохранение изображения
                if (upload != null && upload.ContentLength > 0)
                {
                    string allowExt = Settings.PicTypes;
                    string fileExt = upload.FileName.Substring(upload.FileName.LastIndexOf(".")).Replace(".","").ToLower();

                    if(allowExt.ToLower().Contains(fileExt))
                    {
                        string savePath = Settings.UserFiles + SiteId + "/cart/";
                        backModel.Item.Icon = $"{savePath}{id}.{fileExt}";

                        if (!Directory.Exists(Server.MapPath(savePath)))
                            Directory.CreateDirectory(Server.MapPath(savePath));

                        try
                        {
                            var filePath = Server.MapPath(backModel.Item.Icon);
                            if (System.IO.File.Exists(filePath))
                                System.IO.File.Delete(filePath);

                            upload.SaveAs(filePath);
                        }
                        catch(Exception ex)
                        {
                            //log
                        }
                    }
                }
                #endregion

                if (_cmsRepository.CheckCartCategoryExists(id))
                {
                    _cmsRepository.UpdateCartCategory(backModel.Item);
                    message.Info = "Запись обновлена";
                }
                else
                {
                    _cmsRepository.InsertCartCategory(backModel.Item);
                    message.Info = "Запись добавлена";
                }
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = StartUrl + Request.Url.Query, Text = "вернуться в список" },
                    new ErrorMessageBtnModel { Url = $"{StartUrl}/item/{id}", Text = "ок", Action = "false" }
                };
            }
            else
            {
                message.Info = "Ошибка в заполнении формы. Поля в которых допушены ошибки - помечены цветом";
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = $"{StartUrl}/item/{id}", Text = "ок", Action = "false" }
                };
            }

            //----------------------------------------------

            model.PageName = "Категория товаров и услуг";
            model.Item = _cmsRepository.GetCartCategory(id);
            if (model.Item != null)
            {
                filter = GetFilter();
                var cFilter = FilterModel.Extend<CartFilter>(filter);
                cFilter.CategoryId = id;

                model.ProductsList = _cmsRepository.GetProductsList(cFilter);
                ViewBag.Image = model.Item.Icon;
            }

            model.ErrorInfo = message;
            return View("Item", model);
        }

        [HttpPost]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };

            bool result = _cmsRepository.DeleteCartCategory(id);
            if (result)
            {
                message.Info = "Запись удалена";
                message.Buttons = new ErrorMessageBtnModel[]
                {
                    new ErrorMessageBtnModel { Url = $"{StartUrl}{Request.Url.Query}", Text = "ок", Action = "false" }
                };
            }

            model.ErrorInfo = message;
            return RedirectToAction("Index");
        }

    }
}