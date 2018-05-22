namespace Portal.Areas.Admin.Models
{
    /// <summary>
    /// Ошибка
    /// </summary>
    public class ErrorMessage
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Информация
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// Кнопки
        /// </summary>
        public ErrorMessageBtnModel[] Buttons { get; set; }
    }
}