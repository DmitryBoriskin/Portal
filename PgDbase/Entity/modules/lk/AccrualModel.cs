using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Выставленный счёт
    /// </summary>
    public class InvoiceModel
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
        /// Период
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Номер начисления
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Оплаченность
        /// </summary>
        public bool Payed { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Налог
        /// </summary>
        public decimal? Tax { get; set; }

        /// <summary>
        /// Суммарное потребление
        /// </summary>
        public decimal? Cons { get; set; }

        /// <summary>
        /// Тарифицируемое потребление 1
        /// </summary>
        public decimal? Quantity { get; set; }

        /// <summary>
        /// Тарифицируемое потребление 2
        /// </summary>
        public decimal? Quantity2 { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Тип документа
        /// </summary>
        public string DocType { get; set; }

        /// <summary>
        /// Тип задолженности
        /// </summary>
        public string DebtType { get; set; }

        /// <summary>
        /// Ссылка на платеж
        /// </summary>
        public int? PaymentId { get; set; }

    }
}
