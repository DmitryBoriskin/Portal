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
        public DateTime Send { get; set; }

        /// <summary>
        /// Показание
        /// </summary>
        public decimal Output { get; set; }

        /// <summary>
        /// Тип передачи показания
        /// </summary>
        public GroupsModel DrawlType { get; set; }
    }
}
