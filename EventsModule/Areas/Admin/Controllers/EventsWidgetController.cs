using Portal.Areas.Admin.Controllers;
using PgDbase.entity;
using System;
using System.Web.Mvc;

namespace EventsModule.Areas.Admin.Controllers
{

    public class EventsWidgetController : BaseController
    {
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    base.OnActionExecuting(filterContext);
        //}   

        // GET: Admin/EventsWidget
        public ActionResult Index(Guid id)
        {
            var model = _cmsRepository.GetAttachEventsForNews(id);
            var SelectList = _cmsRepository.GetLastEvents(id);
            if (SelectList != null)
            {
                SelectList Events = new SelectList(SelectList, "Guid", "Title");
                ViewBag.Events = Events;
            }
            ViewBag.NewsId = id;
            return View(model);
        }

        [HttpPost]
        public ActionResult Attach(Guid NewsId, Guid EventId)
        {
            if (_cmsRepository.AttachEventsForNews(NewsId, EventId))
                return Json("Success");
            else
                return Json("Произошла ошибка!");
        }

        [HttpPost]
        public ActionResult DeleteAttach(Guid AttachId)
        {
            if (_cmsRepository.DeleteAttachEventsForNews(AttachId))
                return Json("Success");
            else
                return Json("Произошла ошибка!");
        }
    }
}