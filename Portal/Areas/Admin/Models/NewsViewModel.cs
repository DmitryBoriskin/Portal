﻿using PgDbase.entity;
using PgDbase.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Areas.Admin.Models
{
    public class NewsViewModel : CoreViewModel
    {
        public NewsModel Item { get; set; }
        public Paged<NewsModel> List { get; set; }

    }
}