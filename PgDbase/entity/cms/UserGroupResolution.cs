using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Права группы
    /// </summary>
    public class UserGroupResolution : Resolution
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Группа пользователей
        /// </summary>
        public Guid UserGroup { get; set; }

        /// <summary>
        /// Меню
        /// </summary>
        public Guid Menu { get; set; }
    }
}
