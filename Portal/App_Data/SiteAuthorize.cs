using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Portal
{
    public class SiteAuthorizeAttribute : AuthorizeAttribute
    {
        //protected override bool AuthorizeCore(HttpContextBase httpContext)
        //{
        //    if (base.AuthorizeCore(httpContext))
        //    {
        //        var appUser = httpContext.User as ClaimsPrincipal;
        //        return appUser != null && !appUser.HasClaim("UserDisabled", "True");

        //    }
        //    else
        //    {

        //    }

        //    return false;
        //}

        public string SiteId { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            var userIdentity = filterContext.HttpContext.User.Identity;

            //Идентификация в рамках сайта, если разработчик(у него должны быть все сайты) или администратор(только те, которые он администрирует)
            bool auth = filterContext.HttpContext.User.IsInRole(SiteId);

            //Если вообще не логинился
            if (!userIdentity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }

            //Если доступ запрещен
            if (filterContext.Result is HttpUnauthorizedResult || !auth)
            {
                filterContext.Result = new RedirectResult("~/Account/AccessDenied");

                //filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }
        }
    }
}