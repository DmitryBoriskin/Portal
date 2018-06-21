using LkModule.Areas.Admin.Models;
using PgDbase.entity;
using Portal.Areas.Admin.Controllers;
using Portal.Areas.Admin.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Controllers
{
    public class DepartmentsController : BeCoreController
    {
        FilterModel filter;
        DepartmentViewModel model;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_cmsRepository.ModuleAllowed(ControllerName))
                Response.Redirect("/Admin/");

            model = new DepartmentViewModel()
            {
                PageName = PageName,
                Settings = SettingsInfo,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Sites = _cmsRepository.GetSites(),
                MenuCMS = MenuCmsCore,
                MenuModules = MenuModulCore
            };
        }

        // GET: Admin/Department
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List = _cmsRepository.GetDepartments(filter);
            return View(model);
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
        [MultiButton(MatchFormKey = "action", MatchFormValue = "delete-btn")]
        public ActionResult Delete(Guid id)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };
            bool result = _cmsRepository.DeleteDepartment(id);
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

        [HttpPost]
        public ActionResult Item(Guid id)
        {
            model.Item = _cmsRepository.GetDepartment(id);
            if (model.Item != null)
            {
                ViewBag.XCoord = model.Item.Latitude;
                ViewBag.YCoord = model.Item.Longitude;
                ViewBag.Title = model.Item.Title;
            }
            return View("Item", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [MultiButton(MatchFormKey = "action", MatchFormValue = "save-btn")]
        public ActionResult Save(Guid id, DepartmentViewModel backModel)
        {
            ErrorMessage message = new ErrorMessage
            {
                Title = "Информация"
            };

            if (ModelState.IsValid)
            {
                decimal mapX = 0;
                decimal mapY = 0;
                if (backModel.Item.Latitude != null)
                {
                    mapX = (decimal)backModel.Item.Latitude;
                }
                if (backModel.Item.Longitude != null)
                {
                    mapY = (decimal)backModel.Item.Longitude;
                }

                if (!String.IsNullOrWhiteSpace(backModel.Item.Address) && (mapX == 0 || mapY == 0))
                {
                    string url = $"http://geocode-maps.yandex.ru/1.x/?format=json&results=1&geocode={backModel.Item.Address}";

                    HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                    StreamReader myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream());
                    string html = myStreamReader.ReadToEnd();

                    Regex reCoord = new Regex("(?<=\"Point\":{\"pos\":\")(.*)(?=\"})", RegexOptions.IgnoreCase);

                    string coord = Convert.ToString(reCoord.Match(html).Groups[1].Value);

                    coord = coord.Replace(" ", ";");
                    string[] arrCoord = coord.Split(';');
                    try
                    {
                        mapX = decimal.Parse(arrCoord[1].Replace(".", ","));
                        mapY = decimal.Parse(arrCoord[0].Replace(".", ","));
                    }
                    catch { }
                }
                ViewBag.XCoord = backModel.Item.Latitude = mapX;
                ViewBag.YCoord = backModel.Item.Longitude = mapY;
                ViewBag.Title = backModel.Item.Title;

                backModel.Item.Id = id;
                if (_cmsRepository.CheckDepartmentExists(id))
                {
                    _cmsRepository.UpdateDepartment(backModel.Item);
                    message.Info = "Запись обновлена";
                }
                else
                {
                    _cmsRepository.InsertDepartment(backModel.Item);
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

            model.Item = _cmsRepository.GetDepartment(id);
            model.ErrorInfo = message;
            return View("Item", model);
        }
    }
}