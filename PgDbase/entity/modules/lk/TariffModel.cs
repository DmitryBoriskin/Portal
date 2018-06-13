using System;
using System.Linq;

namespace PgDbase.entity
{
    /// <summary>
    /// Тариф
    /// </summary>
    public class TariffModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime? Begin { get; set; }

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// Запрещённость
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Значения
        /// </summary>
        public TariffValueModel[] Values { get; set; }
    }

    /// <summary>
    /// Значение тарифа
    /// </summary>
    public class TariffValueModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }
    }
}
