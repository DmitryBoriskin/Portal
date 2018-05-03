using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Список групп
    /// </summary>
    public class GroupsModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Псевдоним
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Родитель
        /// </summary>
        public Guid Parent { get; set; }
    }
}
