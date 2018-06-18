using PgDbase.entity;
using PgDbase.Repository.front;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class WidgetController : CoreController
    {
        public ActionResult PageGroup(string alias, string view)
        {
            PageModel[] model = _Repository.GetPageGroup(alias);

            if (String.IsNullOrEmpty(view))
            {
                return PartialView(model);
            }
            else return PartialView(view, model);
        } 
    }
}