using PgDbase.Repository.front;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class PageController : CoreController
    {
        private PageFrontModel model;

        private string _path="/";
        private string _alias="";

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new PageFrontModel();

            #region Определяем путь и алиас
            var url = HttpContext.Request.Url.AbsolutePath.ToLower();
            //Обрезаем  query string (Все, что после ? )
            if (url.LastIndexOf("?") > -1)
                url = url.Substring(0, url.LastIndexOf("?"));
            Regex rgx = new Regex("^/page/");
            url = rgx.Replace(url, "/");

            //Сегменты пути 
            var segments = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (segments != null && segments.Count() > 0)
            {
                _alias = segments.Last();
                if (segments.Count() > 1)
                {
                    _path = string.Join("/", segments.Take(segments.Length - 1));
                    _path = string.Format("/{0}/", _path);
                }
            }
            #endregion
        }

        // GET: Page
        public ActionResult Index()
        { 
            model.Page = _Repository.GetPage(_path,_alias);
            return View(model);
        }
        public ActionResult Group(string alias,string view)
        {
            model.PageGroup = _Repository.GetPageGroup(alias);

            if (String.IsNullOrEmpty(view))
            {
                return View(model);
            }
            else return View(view,model);

        }
        public ActionResult Child()
        {
            model.PageGroup = _Repository.GetPageChild(_path,_alias);
            return View("Group",model);
        }
    }
}