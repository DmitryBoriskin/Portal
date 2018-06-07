using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Фильтр для карты сайта
    /// </summary>
    public class PageFilterModel : FilterModel
    {
        /// <summary>
        /// Родитель
        /// </summary>
        public Guid? Parent { get; set; }
        /// <summary>
        /// Идентификатор группы
        /// </summary>
        public Guid? GroupId { get; set; }
    }
}
