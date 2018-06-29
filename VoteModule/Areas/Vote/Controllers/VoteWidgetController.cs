using Portal.Controllers;
using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using VoteModule.Areas.Vote.Models;

namespace VoteModule.Areas.Vote.Controllers
{
    [Authorize]
    public class VoteWidgetController : CoreController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            //Есть ли у сайта доступ к модулю
            if (!_Repository.ModuleAllowed(ControllerName))
                Response.Redirect("/Page/ModuleDenied");

            //Шаблон
            ViewName = _Repository.GetModuleView(ControllerName, ActionName);
            if (string.IsNullOrEmpty(ViewName))
                throw new Exception("Не указан шаблон представления для данного контроллера и метода");
        }

        // GET: Admin/VoteWidget
        public ActionResult Index()
        {
            VoteWidgetFrontModel model = new VoteWidgetFrontModel();

            model.Item = _Repository.GetVoteForIndexPage();
            if (model.Item != null)
            {
                if (model.Item.Text != null)
                {
                    model.Item.Text = Regex.Replace(model.Item.Text, "<[^>]+>", string.Empty);
                    if (model.Item.Text.Length > 72)
                    {
                        model.Item.Text= model.Item.Text.Substring(0, 72) + "...";
                    }                    
                }                
            }
            return PartialView(ViewName, model);
        }
    }
}