using PgDbase.entity;
using Portal.Models;

namespace LkModule.Areas.Lk.Models
{
    /// <summary>
    /// Модель представления платежей
    /// </summary>
    public class InvoiceFrontModel : LayoutFrontModel
    {
        /// <summary>
        /// Список 
        /// </summary>
        public Paged<InvoiceModel> List { get; set; }

    }

}