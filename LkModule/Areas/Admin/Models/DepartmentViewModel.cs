using PgDbase.entity;
using Portal.Areas.Admin.Models;

namespace LkModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления подразделения
    /// </summary>
    public class DepartmentViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<DepartmentModel> List { get; set; }

        /// <summary>
        /// Единичная запись
        /// </summary>
        public DepartmentModel Item { get; set; }
    }
}