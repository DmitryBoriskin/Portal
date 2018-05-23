using Portal.Areas.Admin.Controllers;
using System;
using System.Web.Mvc;

namespace EventsModule.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("EventsWidget")]
    public class EventsWidgetController : ConnectionController
    {
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    base.OnActionExecuting(filterContext);               
        //}   

        // GET: Admin/EventsWidget
        [Route]
        public ActionResult Index(Guid id)
        {
            var model =_cmsRepository.GetAttachEventsForNews(id);
            var SelectList = _cmsRepository.GetLastEvents(id);
            if (SelectList != null)
            {
                SelectList Events = new SelectList(SelectList, "Guid", "Title");
                ViewBag.Events = Events;
            }
            
            
            ViewBag.NewsId = id;
            return View(model);
        }
        [Route("Attach"), HttpPost]
        public ActionResult Attach(Guid NewsId, Guid EventId)//
        {
            if(_cmsRepository.AttachEventsForNews(NewsId, EventId))            
                return Json("Success");
            else
                return Json("Произошла ошибка!");
        }
        [Route("DeleteAttach"), HttpPost]
        public ActionResult DeleteAttach(Guid AttachId)
        {
            if (_cmsRepository.DeleteAttachEventsForNews(AttachId))
                return Json("Success");
            else
                return Json("Произошла ошибка!");            
        }
    }
}