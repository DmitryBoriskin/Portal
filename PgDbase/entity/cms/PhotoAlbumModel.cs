using System;
using System.Collections.Generic;

namespace PgDbase.entity
{
    /// <summary>
    /// Фотоальбом
    /// </summary>
    public class PhotoAlbumModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Превьюшка
        /// </summary>
        public string Preview { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Запрещённость
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Список изображений
        /// </summary>
        public IEnumerable<PhotoModel> Photos { get; set; }
    }
}
