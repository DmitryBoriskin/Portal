using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System;
using System.Linq;

namespace LkModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления приборов учёта
    /// </summary>
    public class PuViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<PuModel> List { get; set; }
    }
}