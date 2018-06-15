using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PgDbase.Entity.common;
using PgDbase.Repository.front;
using Portal.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class LayoutController : CoreController
    {
        protected LayoutModel _layoutmodel;
        protected ApplicationUser _user;
        protected string _path = "/";
        protected string _alias = "";

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);


            _layoutmodel = _Repository.GetLayoutInfo();

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


            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            _user = manager.FindById(User.Identity.GetUserId());

        }      
    }
}