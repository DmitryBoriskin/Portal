using PgDbase;
using PgDbase.Repository.cms;
using Portal.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventsModule.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("EventsWidget")]
    public class EventsWidgetController : ConnectionController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);               
        }   

        // GET: Admin/EventsWidget
        [Route]
        public ActionResult Index(Guid id)
        {
            _cmsRepository.GetAttachEventsForNews(id);
            return View();
        }
    }
}