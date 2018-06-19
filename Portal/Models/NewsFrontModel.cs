using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class NewsFrontModel: LayoutFrontModel
    {
        public Paged<NewsModel> List { get; set; }
        public NewsModel Item { get; set; }
    }
}