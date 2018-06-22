using PgDbase.entity;
using Portal.Models;
using System.Collections.Generic;

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

    public class SubscrWidgetFrontModel
    {
        /// <summary>
        /// Лицевой счет по умолчанию 
        /// </summary>
        public SubscrShortModel Item { get; set; }

        // <summary>
        /// Список 
        /// </summary>
        public List<SubscrShortModel> List { get; set; }

    }

}