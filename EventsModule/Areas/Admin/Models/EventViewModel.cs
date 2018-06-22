using PgDbase.entity;
using Portal.Areas.Admin.Models;

namespace EventsModule.Areas.Admin.Models
{
    public class EventViewModel: CoreViewModel
    {
        public Paged<EventModel> List { get; set; }
        public EventModel Item { get; set; }
    }
}