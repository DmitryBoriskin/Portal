﻿using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Areas.Admin.Models
{
    public class MenuViewModel : CoreViewModel
    {

        /// <summary>
        /// Меню админки
        /// </summary>
        public CmsMenuModel[] MenuList { get; set; }
        public CmsMenuItem MenuItem { get; set; }
        public CmsMenuModel[] MenuGroup { get; set; }
    }
}