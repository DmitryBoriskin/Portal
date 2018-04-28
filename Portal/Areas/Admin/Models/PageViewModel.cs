using PgDbase.entity;

namespace Portal.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления карты сайты
    /// </summary>
    public class PageViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public PageModel[] List { get; set; }

        /// <summary>
        /// Единичная запись
        /// </summary>
        public PageModel Item { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterModel Filter { get; set; }

        /// <summary>
        /// Хлебные крошки
        /// </summary>
        public BreadCrumb BreadCrumbs { get; set; }
    }
}