using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class InfoFrontModel:LayoutFrontModel
    {
        public SubscrModel DefaultSubscrInfo { get; set; }
        //public PageModel Page { get; set; }
        //public PageModel[] PageGroup { get; set; }
    }
}