﻿using PgDbase.entity;
using PgDbase.Repository.front;
using Portal.Models;
using System;
using System.IO;
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

            model = new NewsFrontModel()
            {
                LayoutInfo = _layoutData,
                PageName = _pageName,
                User = CurrentUser,
                Breadcrumbs = _breadcrumb
            };
        }

        // GET: News
        public ActionResult Index()
        {            
            filter = GetFilter();
            filter.Category = "news";
            model.List = _Repository.GetNewsList(filter);
            if(model.List!=null)
                if(model.List.Items!=null)
                    foreach (var item in model.List.Items)
                    {
                        if (item.Photo != null)
                        {
                            var f = new FileInfo(Server.MapPath(item.Photo));
                            item.Photo = (f.Exists) ? item.Photo : null;
                        }                        
                    }
            return View(model);
        }


        public ActionResult NewsItem(string path)
        {
            ViewBag.Title=model.PageName = "Новости";
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
                    if (model.Item.Photo != null)
                    {
                        var f = new FileInfo(Server.MapPath(model.Item.Photo));
                        model.Item.Photo = (f.Exists) ? model.Item.Photo : null;
                    }
                    
                    model.Breadcrumbs = _Repository.GetBreadCrumbCollection("news", "/");
                    return View(model);
                }
            }
            return HttpNotFound();
        }
    }
}