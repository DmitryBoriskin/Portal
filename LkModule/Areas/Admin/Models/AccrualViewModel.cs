using PgDbase.entity;
using Portal.Areas.Admin.Models;

namespace LkModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления выставленных счетов
    /// </summary>
    public class AccrualViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<AccrualModel> List { get; set; }

        /// <summary>
        /// Единичная запись счёта
        /// </summary>
        public AccrualModel Item { get; set; }
    }
}