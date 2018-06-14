using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PgDbase.Entity.common;
using PgDbase.Repository.front;
using Portal.Models;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class LayoutController : CoreController
    {
        protected LayoutModel _layoutmodel;
        protected ApplicationUser _user;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _layoutmodel = _Repository.GetLayoutInfo();
            var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            _user = manager.FindById(User.Identity.GetUserId());

        }      
    }
}