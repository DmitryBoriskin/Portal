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
        public int PeriodId { get; set; }

        /// <summary>
        /// Период в формате DateTime
        /// </summary>
        public DateTime Period
        {
            get
            {
                var str = PeriodId.ToString();
                var year = str.Substring(0, 4);
                var month = str.Substring(4, 1) == "0" ? str.Substring(5, 1) : str.Substring(4, 2);

                return new DateTime(int.Parse(year), int.Parse(month), 1, 0, 0, 0);
            }
        }

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
