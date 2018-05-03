using PgDbase.entity;

namespace Portal.Areas.Admin.Models
{
    /// <summary>
    /// Фильтр в виде дерева
    /// </summary>
    public class FilterTreeModel
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Ссылка
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Иконка
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Только чтение
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Ссылка на редактирование
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Название кнопки
        /// </summary>
        public string BtnName { get; set; }

        /// <summary>
        /// Эл-ты 
        /// </summary>
        public CatalogList[] Items { get; set; }
    }
}