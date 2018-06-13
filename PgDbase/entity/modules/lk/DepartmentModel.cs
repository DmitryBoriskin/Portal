using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Подразделение
    /// </summary>
    public class DepartmentModel
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
        /// Адрес
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Режим работы
        /// </summary>
        public string WorkTime { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Широта
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Запрещённость
        /// </summary>
        public bool Disabled { get; set; }
    }
}
