using PgDbase.Entity.common;
using PgDbase.Repository.front;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class LayoutController : CoreController
    {
        protected LayoutModel _layoutmodel;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _layoutmodel = _Repository.GetLayoutInfo();
        }      
    }
}