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
        /// Статус платежа
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPeni { get; set; }

        /// <summary>
        /// Тип платежа
        /// </summary>
        public string Type { get; set; }
    }
}
