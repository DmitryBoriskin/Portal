using PgDbase.entity;
using Portal.Areas.Admin.Models;

namespace LkModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления выставленных счетов
    /// </summary>
    public class ChargesViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<ChargeModel> List { get; set; }

        /// <summary>
        /// Единичная запись счёта
        /// </summary>
        public ChargeModel Item { get; set; }
    }
}