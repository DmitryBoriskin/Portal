using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Выставленный счёт
    /// </summary>
    public class StatisticsModel
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
        public DateTime? Period {
            get
            {
                if (PeriodId.HasValue)
                {
                    var str = PeriodId.Value.ToString();
                    var year = str.Substring(0, 4);
                    var month = str.Substring(4, 1) == "0" ? str.Substring(5, 1) : str.Substring(4, 2);

                    return new DateTime(int.Parse(year), int.Parse(month), 1, 0, 0, 0);
                }
                return null;
            }
        }


        /// <summary>
        /// Cумма по итоговой счет фактуре
        /// </summary>
        public decimal? InvoiceAmount { get; set; }

        /// <summary>
        /// Сумма по платежам
        /// </summary>
        public decimal? PaymentAmount { get; set; }

        /// <summary>
        /// Сумма по потреблению по группе КВт*ч
        /// </summary>
        public decimal? ConsumptionAmount1 { get; set; }

        /// <summary>
        /// Сумма по потреблению по группе Квт
        /// </summary>
        public decimal? ConsumptionAmount2 { get; set; }

    }
}
