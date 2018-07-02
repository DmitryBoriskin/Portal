using PgDbase.entity;
using Portal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VoteModule.Areas.Vote.Models;

namespace VoteModule.Areas.Vote.Controllers
{
    [Authorize]
    public class VoteController : LayoutController
    {
        FilterModel filter;
        VoteFrontModel model;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            //Есть ли у сайта доступ к модулю
            //if (!_Repository.ModuleAllowed(ControllerName))
            //    Response.Redirect("/Page/ModuleDenied");

            model = new VoteFrontModel()
            {
                LayoutInfo = _layoutData,
                Breadcrumbs = _breadcrumb,
                PageName = _pageName,
                User = CurrentUser
            };



        }

        // GET: Vote/Vote
        public ActionResult Index()
        {
            filter = GetFilter();
            model.List = _Repository.GetVoteList(filter);

            return View(model);
        }
        public ActionResult Item(Guid id)
        {
            model.Item = _Repository.GetVoteItem(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult VoteAction(VoteFrontModel backm)
        {
            var variant = Request["variant"];
            if (!String.IsNullOrEmpty(variant))
            {
                string[] AnswerArr = variant.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
                _Repository.ActionVote(backm.Item.Id, AnswerArr);
            }

            //return View("Item",model);
            return Redirect("/vote/item/" + backm.Item.Id);
        }
    }
}