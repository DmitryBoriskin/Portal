using PgDbase.entity;

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
}