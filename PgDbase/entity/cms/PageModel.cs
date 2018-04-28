using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Эл-т карты сайта
    /// </summary>
    public class PageModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Родительский идентификатор
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// Путь
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Псевдоним
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Приоритет
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Запрещённость
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Ключевые слова
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Идентификатор контроллера
        /// </summary>
        public int? SiteController { get; set; }

        /// <summary>
        /// Кол-во дочерних эл-тов
        /// </summary>
        public int CountChilds { get; set; }

        /// <summary>
        /// Удаляемость
        /// </summary>
        public bool IsDeleteble { get; set; }

        /// <summary>
        /// Дочерние эл-ты
        /// </summary>
        public PageModel[] Childrens { get; set; }
    }
}
