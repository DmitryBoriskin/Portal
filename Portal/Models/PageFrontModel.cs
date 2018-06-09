using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class PageFrontModel
    {
        public PageModel Page { get; set; }
        public PageModel[] PageGroup { get; set; }
    }
}