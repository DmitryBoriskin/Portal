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

        /// <summary>
        /// Назначение платежа
        /// </summary>
        public string Destination { get; set; }


        /// <summary>
        /// Связанные Документы
        /// </summary>
        public DocumentModel[] Documents { get; set; }


    }

    public class DocumentModel
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Type { get; set; }

        public string Number { get; set; }

    }
}
