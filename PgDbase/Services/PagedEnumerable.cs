using System.Collections.Generic;

namespace PgDbase.Services
{
    /// <summary>
    /// Постраничный вывод сущностей
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedEnumerable<T>
    {
        /// <summary>
        /// Список сущностей
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Пейджер
        /// </summary>
        public Pager Pager { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="items"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNum"></param>
        /// <param name="totalCount"></param>
        public PagedEnumerable(IEnumerable<T> items, int pageSize, int pageNum, int totalCount)
        {
            Items = items;
            Pager = new Pager(pageSize, pageNum, totalCount);
        }
    }
}
