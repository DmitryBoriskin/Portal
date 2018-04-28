using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class ModuleModel
    {
        /// <summary>
        /// Идентификатор в бд
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор родителя
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Имя модуля
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Имя контроллера
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Имя Экшена
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Вью, если не указана то берется по умолчанию
        /// </summary>
        public Guid View { get; set; }

    }
}
