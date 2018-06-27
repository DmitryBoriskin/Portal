using Portal.Controllers;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using VoteModule.Areas.Vote.Models;

namespace VoteModule.Areas.Vote.Controllers
{
    [Authorize]
    public class VoteWidgetController : CoreController
    {   
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
            return PartialView(model);
        }
    }
}