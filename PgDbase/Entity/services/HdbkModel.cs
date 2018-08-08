using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Стандартный Справочник ключ-значение
    /// </summary>
    public class HdbkModel
    {
        /// <summary>
        /// Идентификатор, ключ
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Наименование - значение
        /// </summary>
        public string Name { get; set; }
    }
}
