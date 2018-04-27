using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Areas.Admin.Models
{
    public class SitesViewModel : CoreViewModel
    {
        /// <summary>
        /// Список сайтов
        /// </summary>
        public SitesList List { get; set; }

        /// <summary>
        /// Единичная запись сайта
        /// </summary>
        public SitesModel Item { get; set; }

        /// <summary>
        /// Список типов
        /// </summary>
        public SelectList TypeList { get; set; }

        /// <summary>
        /// Список организаций
        /// </summary>
        public SelectList OrgsList { get; set; }

        /// <summary>
        /// Список событий
        /// </summary>
        public SelectList EventsList { get; set; }

        /// <summary>
        /// Список людей
        /// </summary>
        public SelectList MainSpecialistList { get; set; }

        /// <summary>
        /// Список тем
        /// </summary>
        public SelectList Themes { get; set; }        
    }
}