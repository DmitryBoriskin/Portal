using PgDbase.entity;
using Portal.Models;

namespace EventsModule.Areas.Events.Models
{
    /// <summary>
    /// Модель представления платежей
    /// </summary>
    public class EventFrontModel : LayoutFrontModel
    {
        /// <summary>
        /// Список 
        /// </summary>
        public Paged<EventModel> List { get; set; }

    }

}