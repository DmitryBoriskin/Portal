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

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="items"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="totalCount"></param>
        public Paged(IEnumerable<T> items, int pageSize, int pageNum, int totalCount)
        {
            Items = items;
            Pager = new PagerModel(pageSize, pageNum, totalCount);
        }
    }
}
