using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Фильтр
    /// </summary>
    public class FilterModel
    {
        /// <summary>
        /// Страница
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Кол-во эл-тов на странице
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Флаг запрещённости
        /// </summary>
        public bool? Disabled { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Группа
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Строка поиска
        /// </summary>
        public string SearchText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid? SiteId { get; set; }
        /// <summary>
        /// Язык
        /// </summary>
        public string Lang { get; set; }

        public static T Extend<T>(FilterModel f)
            where T : FilterModel, new()
        {
            return new T()
            {
                Page = f.Page,
                Size = f.Size,
                Disabled = f.Disabled,
                Type = f.Type,
                Category = f.Category,
                SiteId = f.SiteId,
                Group = f.Group,
                Date = f.Date,
                DateEnd = f.DateEnd,
                SearchText = f.SearchText,
                Lang = f.Lang
            };
        }
    }
}
