using System;
using System.Linq;

namespace PgDbase.entity
{
    /// <summary>
    /// Показание ПУ
    /// </summary>
    public class MeterModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Дата передачи
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Дата передачи предыдущая
        /// </summary>
        public DateTime? DatePrev { get; set; }

        /// <summary>
        /// Показание
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Потребление
        /// </summary>
        public decimal? Const { get; set; }

        /// <summary>
        /// Потребление
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// Показания за год
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Показания за месяц
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Дней прошло с пред показаний
        /// </summary>
        public int? Days { get; set; }

        /// <summary>
        /// Тип передачи показания
        /// </summary>
        public string DeliveryMethod { get; set; }
    }
}
