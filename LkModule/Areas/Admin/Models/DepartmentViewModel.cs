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
        public Paged<Department> List { get; set; }

        /// <summary>
        /// Единичная запись
        /// </summary>
        public Department Item { get; set; }
    }
}