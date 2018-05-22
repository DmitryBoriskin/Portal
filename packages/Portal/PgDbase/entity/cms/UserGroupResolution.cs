using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Права группы
    /// </summary>
    public class UserGroupResolution : ResolutionModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Группа пользователей
        /// </summary>
        public Guid UserGroup { get; set; }

        /// <summary>
        /// Меню
        /// </summary>
        public GroupsModel Menu { get; set; }
    }
}
