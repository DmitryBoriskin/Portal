using PgDbase.entity;
using PgDbase.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Models
{
    /// <summary>
    /// модель представления для представления
    /// </summary>
    public class SitesViewModel : CoreViewModel
    {
        /// <summary>
        /// Постраничный список
        /// </summary>
        public PagedEnumerable<SitesModel> List { get; set; }

        /// <summary>
        /// Сайт
        /// </summary>
        public SitesModel Item { get; set; }


    }
}