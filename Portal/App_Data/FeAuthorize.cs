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
    public class FeAuthorizeAttribute : AuthorizeAttribute
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

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                //filterContext.Result = new RedirectResult("~/Account/AccessDenied");

                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }
        }
    }
}