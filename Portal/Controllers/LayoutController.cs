﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PgDbase.entity;
using PgDbase.Entity.common;
using PgDbase.Repository.front;
using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Portal.Controllers
{
    [Authorize]
    public class LayoutController : CoreController
    {
        protected LayoutModel _layoutData;
        protected List<Breadcrumbs> _breadcrumb;
        protected string _path = "/";
        protected string _alias = "";
        protected string _pageName = "";
        protected string _Controller = "";
        protected string _Action = "";

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.Layout = "/sites/rushydro/view/_Layout.cshtml";


            _Controller = (String)RouteData.Values["controller"].ToString().ToLower();
            _Action = RouteData.Values["action"].ToString().ToLower();

            var _PageModel= _Repository.GetInfoPageModule(_Controller, _Action);
            

            if (_PageModel != null)
            {
                _alias = _PageModel.Alias;
                _path = _PageModel.Path;
            }
            else
            {
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

            //хлебные крошки
            _breadcrumb = _Repository.GetBreadCrumbCollection(_alias, _path);
            _pageName = ViewBag.Title = _breadcrumb.Last().Title;
            ViewBag.ThisUrl = _breadcrumb.Last().Url;
        }
    }
}