using PgDbase.entity;
using PgDbase.Repository.front;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class NewsController : LayoutController
    {
        private NewsFrontModel model;
        private FilterModel filter;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            model = new NewsFrontModel() {
                LayoutInfo = _layoutmodel
            };
        }
        // GET: News
        public ActionResult Index()
        {
            filter = GetFilter();            
            model.List = _Repository.GetNewsList(filter);
            return View(model);
        }
        public ActionResult NewsItem(string path)
        {
            var n = path.IndexOf("-");
            if (n > 0)
            {
                path = path.Substring(0, n);
            }
            if (Int32.TryParse(path, out int number))
            {
                model.Item = _Repository.GetNewsItem(number);
                if (model.Item != null)
                {
                    return View(model);
                }
            }
            return HttpNotFound();
        }
    }
}