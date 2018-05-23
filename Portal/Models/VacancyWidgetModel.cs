using PgDbase.entity;

namespace Portal.Models
{
    /// <summary>
    /// Модель вакансии для модуля
    /// </summary>
    public class VacancyWidgetModel : WidgetCoreModel
    {
        /// <summary>
        /// Список вакансий
        /// </summary>
        public VacancyModel[] List { get; set; }
    }
}