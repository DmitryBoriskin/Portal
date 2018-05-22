namespace PgDbase.entity
{
    /// <summary>
    /// Хлебная крошка
    /// </summary>
    public class BreadCrumb
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Страница по умолчанию
        /// </summary>
        public string DefaultUrl { get; set; }

        /// <summary>
        /// Элементы
        /// </summary>
        public GroupsModel[] Items { get; set; }
    }
}
