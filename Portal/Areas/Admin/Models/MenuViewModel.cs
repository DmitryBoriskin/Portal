using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Areas.Admin.Models
{
    public class MenuViewModel : CoreViewModel
    {

        public CmsMenuItemModel[] MenuGroups { get; set; }
        public CmsMenuItemModel[] MenuList { get; set; }
        public CmsMenuItemModel Item { get; set; }

    }
}