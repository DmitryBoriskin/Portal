using PgDbase.entity;
using PgDbase.Entity.common;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Models
{
    public class LayoutViewModel
    {
        public LayoutModel LayoutInfo { get; set; }
        public ApplicationUser User { get; set; }
    }
}