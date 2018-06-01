using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Web;

namespace Portal.Areas.Admin.Models
{
    /// <summary>
    /// Модель для представления
    /// </summary>
    public class PhotoViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<PhotoAlbumModel> List { get; set; }

        /// <summary>
        /// Единичная запись
        /// </summary>
        public PhotoAlbumModel Item { get; set; }
    }

    /// <summary>
    /// Помощник для сохранения изображений
    /// </summary>
    public class UploadImageHelper
    {
        /// <summary>
        /// Загружаемые файлы
        /// </summary>
        public IEnumerable<HttpPostedFileBase> Uploads { get; set; }

        /// <summary>
        /// Путь
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Идентификатор альбома
        /// </summary>
        public Guid AlbumId { get; set; }

        /// <summary>
        /// Необходимость превью для альбома
        /// </summary>
        public bool IsNeedPreview { get; set; }
    }
}