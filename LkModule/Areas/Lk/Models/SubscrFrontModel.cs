using PgDbase.entity;
using Portal.Models;


namespace LkModule.Areas.Lk.Models
{
    /// <summary>
    /// Модель представления платежей
    /// </summary>
    public class SubscrFrontModel : LayoutFrontModel
    {
        /// <summary>
        /// Список 
        /// </summary>
        public SubscrModel Item { get; set; }

    }

}