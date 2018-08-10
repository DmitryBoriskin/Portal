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
        ///  линк показания в ОМНИС
        /// </summary>
        public long Link { get; set; }

        /// <summary>
        /// Идентификатор устройства, подключенного к лс
        /// </summary>
        public Guid DeciceId { get; set; }

        /// <summary>
        ///  Идентификатор устройства, подключенного к лс  в ОМНИС
        /// </summary>
        public long DeviceLink { get; set; }

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
        /// Суммарное потребление
        /// </summary>
        public decimal? Const { get; set; }

        /// <summary>
        /// Тарифицируемое потребление
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

        /// <summary>
        /// Тарифная зона
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Тип измеряемой энергии
        /// </summary>
        public string EnergyType { get; set; }

        /// <summary>
        /// Вид измеряемой энергии, например АЭ-
        /// </summary>
        public string EnergyTypeName { get; set; }

        /// <summary>
        /// Ид. Статус
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public int Status { get; set; }

    }
}
