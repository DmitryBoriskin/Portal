using PgDbase.Repository.front;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class CoreController : Controller
    {
        /// <summary>
        /// Репозиторий для работы с сущностями
        /// </summary>
        protected Repository _Repository { get; private set; }

        public CoreController()
        {
            _Repository = new Repository("dbConnection");
        }
        // GET: Core
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Возвращает JSON фотоальбома
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult GetPhotoGallery(Guid id)
        {
            var data = _Repository.GetGallery(id);
            if (data != null)
                return Json(data, JsonRequestBehavior.AllowGet);
            else
                return Json(null, JsonRequestBehavior.AllowGet);            
        }
    }
}