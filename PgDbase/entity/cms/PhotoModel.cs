using System;

namespace PgDbase.entity
{
    /// <summary>
    /// Изображение костяк
    /// </summary>
    public class PhotoCoreModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Альбом
        /// </summary>
        public Guid Album { get; set; }
        
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Изображение
    /// </summary>
    public class PhotoModel : PhotoCoreModel
    {
        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Ссылка на изображение
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Превьюшка
        /// </summary>
        public string Preview { get; set; }

        /// <summary>
        /// Сортировка
        /// </summary>
        public int Sort { get; set; }
    }
}
