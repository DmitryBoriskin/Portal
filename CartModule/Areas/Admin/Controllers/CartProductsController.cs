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
    public class CartProductsController : BeCoreController
    {
        FilterModel filter;
        CartProductViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_cmsRepository.ModuleAllowed(ControllerName))
                Response.Redirect("/Admin/");

            model = new CartProductViewModel()
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

            model.Categories = _cmsRepository.GetCartCategoriesList(new CartFilter() { });
        }

        // GET: Admin/Lk
        public ActionResult Index()
        {
            model.PageName = "Товары и услуги";
            filter = GetFilter();
            model.Filter = filter;
            var cFilter = FilterModel.Extend<CartFilter>(filter);
            model.List = _cmsRepository.GetProducts(cFilter);
           
            return View(model);
        }

        public ActionResult Item(Guid id)
        {
            model.PageName = "Товары и услуги";
            model.Item = _cmsRepository.GetProduct(id);
            if (model.Item != null && model.Item.Images != null && model.Item.Images.Count() > 0)
                ViewBag.Image = model.Item.Images[0].Path;

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
        public ActionResult Search(string searchtext, string category, bool enabled, string size)
        {
            string query = HttpUtility.UrlDecode(Request.Url.Query);
            query = AddFilterParam(query, "searchtext", searchtext);
            query = AddFilterParam(query, "category", category);
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
        public ActionResult Save(Guid id, CartProductViewModel backModel, HttpPostedFileBase upload)
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
                    string fileExt = upload.FileName.Substring(upload.FileName.LastIndexOf(".")).Replace(".", "").ToLower();

                    if (allowExt.ToLower().Contains(fileExt))
                    {
                        string savePath = Settings.UserFiles + SiteId + "/cart/" + id + "/";
                        backModel.Item.Images[0] = new ProductImage()
                        {
                            Id = Guid.NewGuid(),
                            ProductId = id,
                            Title = "",
                            IsMain = true,
                            Path = $"{savePath}{id}.{fileExt}"
                        };

                        if (!Directory.Exists(Server.MapPath(savePath)))
                            Directory.CreateDirectory(Server.MapPath(savePath));

                        try
                        {
                            var filePath = Server.MapPath(backModel.Item.Images[0].Path);
                            if (System.IO.File.Exists(filePath))
                                System.IO.File.Delete(filePath);

                            upload.SaveAs(filePath);
                        }
                        catch (Exception ex)
                        {
                            //log
                        }
                    }
                }
                #endregion


                if (_cmsRepository.CheckCartProductExists(id))
                {
                    _cmsRepository.UpdateCartProduct(backModel.Item);
                    message.Info = "Запись обновлена";
                }
                else
                {
                    _cmsRepository.InsertCartProduct(backModel.Item);
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

            model.Item = _cmsRepository.GetProduct(id);
            if (model.Item != null && model.Item.Images != null && model.Item.Images.Count() > 0)
                ViewBag.Image = model.Item.Images[0].Path;

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

            bool result = _cmsRepository.DeleteCartProduct(id);
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