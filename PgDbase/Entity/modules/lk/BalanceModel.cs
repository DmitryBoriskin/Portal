using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Выставленный счёт
    /// </summary>
    public class DebitCreditModel
    {
        /// <summary>
        /// Линк в ОМНИС
        /// </summary>
        public Guid? SubscrId { get; set; }

        /// <summary>
        /// Линк в ОМНИС
        /// </summary>
        public long? Subscr { get; set; }

        /// <summary>
        /// Лицевой счет
        /// </summary>
        public int? PeriodId { get; set; }

        /// <summary>
        /// Дата начала периода
        /// </summary>
        public DateTime? Period { get; set; }


        /// <summary>
        /// Cумма по итоговой счет фактуре
        /// </summary>
        public decimal? InvoiceAmount { get; set; }

        /// <summary>
        /// Сумма по платежам
        /// </summary>
        public decimal? PaymentAmount { get; set; }

    }
}
