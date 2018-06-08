using System;
using System.Linq;

namespace PgDbase.entity
{
    /// <summary>
    /// Платёж
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Статус платежа
        /// </summary>
        public GroupsModel Status { get; set; }

        /// <summary>
        /// Тип платежа
        /// </summary>
        public GroupsModel Type { get; set; }
    }
}
