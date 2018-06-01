using PgDbase.entity;
using Portal.Areas.Admin.Models;

namespace LkModule.Areas.Admin.Models
{
    /// <summary>
    /// Модель представления лицевого счёта
    /// </summary>
    public class SubscrViewModel : CoreViewModel
    {
        /// <summary>
        /// Список
        /// </summary>
        public Paged<Subscr> List { get; set; }

        /// <summary>
        /// Единичная запись
        /// </summary>
        public Subscr Item { get; set; }
    }
}