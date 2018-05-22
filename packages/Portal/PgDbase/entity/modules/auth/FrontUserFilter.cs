using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Фильтр
    /// </summary>
    public class FrontUserFilter : FilterModel
    {
        /// <summary>
        /// Сайт
        /// </summary>
        public Guid? Site { get; set; }
    }
}
