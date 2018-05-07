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
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор модуля-родителя
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Имя модуля
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Имя контроллера
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// Имя Экшена
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// Вью
        /// </summary>
        public Guid View { get; set; }

        /// <summary>
        /// Описание модуля
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Составляющие части модуля
        /// </summary>
        public ModuleModel[] ModuleParts { get; set; }
    }

}
