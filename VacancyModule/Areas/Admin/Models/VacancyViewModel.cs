using PgDbase.entity;
using Portal.Areas.Admin.Models;

namespace VacancyModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель для представления
    /// </summary>
    public class VacancyViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<VacancyModel> List { get; set; }

        /// <summary>
        /// Единичная запись
        /// </summary>
        public VacancyModel Item { get; set; }
    }
}