using System;
using System.ComponentModel.DataAnnotations;

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
        [RegularExpression(@"^[^-]([a-zA-Z0-9-]+)$", ErrorMessage = "Поле «Алиас» может содержать только буквы латинского алфавита и символ - (дефис). Оно не может начинаться с дефиса.")]
        public string Alias { get; set; }
        public string Section { get; set; }

        /// <summary>
        /// Родитель
        /// </summary>
        public Guid? Parent { get; set; }
    }
}
