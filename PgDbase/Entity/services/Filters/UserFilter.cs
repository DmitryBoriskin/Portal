using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Фильтр
    /// </summary>
    public class UserFilter : FilterModel
    {
        /// <summary>
        /// Фильтрация в соответсвии справами
        /// </summary>
        public String[] ExcludeRoles { get; set; }

    }
}
