using System;
using System.Linq;

namespace PgDbase.entity
{
    /// <summary>
    /// Прибор учёта
    /// </summary>
    public class MeterDevice
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Номер
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Марка
        /// </summary>
        public string Mark { get; set; }

        /// <summary>
        /// Дата установки
        /// </summary>
        public DateTime InstallDate { get; set; }

        /// <summary>
        /// Место установки
        /// </summary>
        public string InstallPlace { get; set; }

        /// <summary>
        /// Запрещённость
        /// </summary>
        public bool Disabled { get; set; }
    }
}
