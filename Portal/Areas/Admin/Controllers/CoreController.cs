using PgDbase;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Controllers
{
    [Authorize]
    public class CoreController : Controller
    {
        /// <summary>
        /// Репозитория для работы с сущностями cms
        /// </summary>
        protected CmsRepository CmsRepository { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public CoreController()
        {
            CmsRepository = new CmsRepository();
        }

        // GET: Admin/Core
        public ActionResult Index()
        {
            return View();
        }
    }
}