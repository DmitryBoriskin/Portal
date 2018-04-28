namespace PgDbase.entity
{
    /// <summary>
    /// Справочник
    /// </summary>
    public class CatalogList : GroupsModel
    {
        /// <summary>
        /// Иконка
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Ссылка для применения фильтра
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Ссылка на редактирование группы
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Выбранность
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
