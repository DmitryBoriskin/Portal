using System.ComponentModel;

namespace Portal.Areas.Admin.Models
{
    /// <summary>
    /// Сообщение об ошибках
    /// </summary>
    public class ErrorMessageBtnModel
    {
        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Тексь
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Стиль
        /// </summary>
        [DefaultValue("default")]
        public string Style { get; set; }

        /// <summary>
        /// Действие
        /// </summary>
        public string Action { get; set; }
    }
}