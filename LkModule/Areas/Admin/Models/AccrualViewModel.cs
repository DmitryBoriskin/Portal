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
        public Paged<InvoiceModel> List { get; set; }

        /// <summary>
        /// Единичная запись счёта
        /// </summary>
        public InvoiceModel Item { get; set; }
    }
}