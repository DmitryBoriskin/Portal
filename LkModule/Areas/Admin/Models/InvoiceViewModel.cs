using PgDbase.entity;
using Portal.Areas.Admin.Models;

namespace LkModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления выставленных счетов
    /// </summary>
    public class InvoiceViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<InvoiceModel> List { get; set; }

        /// <summary>
        /// Единичная запись счёта
        /// </summary>
        public InvoiceModel Item { get; set; }

        /// <summary>
        /// фильтр
        /// </summary>
        public FilterModel Filter { get; set; }
    }
}