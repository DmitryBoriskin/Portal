using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Фильтр
    /// </summary>
    public class TemplateFilter : FilterModel
    {
        /// <summary>
        /// Фильтрация шаблонов, доступных для конкретного контроллера
        /// </summary>
        public Guid? Controller { get; set; }

    }
}
