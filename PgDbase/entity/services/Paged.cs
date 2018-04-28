using System.Collections.Generic;

namespace PgDbase.entity
{
    /// <summary>
    /// Постраничный вывод сущностей
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Paged<T>
    {
        /// <summary>
        /// Список сущностей
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Пейджер
        /// </summary>
        public PagerModel Pager { get; set; }
    }
}
