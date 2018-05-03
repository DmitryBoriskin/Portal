using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Шаблон/представление/Вьюха
    /// </summary>
    public class TemplateModel
    {
        /// <summary>
        /// Идентификатор в бд
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Имя шаблона
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Для какого контроллера предназначен шаблон
        /// </summary>
        public ModuleModel Controller { get; set; }

        /// <summary>
        /// Путь к шаблону
        /// </summary>
        public string ViewPath { get; set; }

        /// <summary>
        /// Картинка, показывающяя как выглядит шаблон
        /// </summary>
        public string Image { get; set; }

    }
}
