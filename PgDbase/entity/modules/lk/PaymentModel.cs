using System;
using System.Linq;

namespace PgDbase.entity
{
    /// <summary>
    /// Платёж
    /// </summary>
    public class PaymentModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Период
        /// </summary>
        public int Period { get; set; }

    }
}
