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
    public class LayoutFrontModel
    {
        public LayoutModel LayoutInfo { get; set; }

        /// <summary>
        /// Хлебные крошки
        /// </summary>
        public List<Breadcrumbs> Breadcrumbs { get; set; }

        /// <summary>
        /// Название страницы
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public ApplicationUser User { get; set; }

    }
}