using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace LkModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления лицевого счёта
    /// </summary>
    public class SubscrViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<Subscr> List { get; set; }

        /// <summary>
        /// Единичная запись
        /// </summary>
        public Subscr Item { get; set; }

        /// <summary>
        /// Подразделения
        /// </summary>
        public IEnumerable<SelectListItem> Departments { get; set; }
    }
}