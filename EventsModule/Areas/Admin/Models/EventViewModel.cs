using PgDbase.entity;
using Portal.Areas.Admin.Models;

namespace EventsModule.Areas.Admin.Models
{
    public class EventViewModel: CoreViewModel
    {
        public Paged<EventsModel> List { get; set; }
        public EventsModel Item { get; set; }
    }
}