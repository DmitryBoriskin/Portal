using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Выставленный счёт
    /// </summary>
    public class AccrualModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        public decimal Debt { get; set; }

        /// <summary>
        /// Оплаченность
        /// </summary>
        public bool Payed { get; set; }
    }
}
