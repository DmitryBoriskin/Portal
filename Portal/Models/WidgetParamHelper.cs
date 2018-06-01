namespace Portal.Models
{
    /// <summary>
    /// Параметры для вьюхи виджета
    /// </summary>
    public class WidgetParamHelper
    {
        /// <summary>
        /// Название вьюхи
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Кол-во эл-тов
        /// </summary>
        public int Count { get; set; }
    }
}