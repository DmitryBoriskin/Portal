using PgDbase.entity;
using Portal.Models;


namespace LkModule.Areas.Lk.Models
{
    /// <summary>
    /// Модель представления платежей
    /// </summary>
    public class PaymentFrontModel : LayoutFrontModel
    {
        /// <summary>
        /// Список 
        /// </summary>
        public Paged<PaymentModel> List { get; set; }
        /// <summary>
        /// Сумма за выбранный период
        /// </summary>
        public decimal SummaZaPeriod { get; set; }

    }

}