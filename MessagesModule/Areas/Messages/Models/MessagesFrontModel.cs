using PgDbase.entity;
using Portal.Models;

namespace MessagesModule.Areas.Messages.Models
{
    public class MessagesFrontModel: LayoutFrontModel
    {
        //public Paged<MessagesModel> List { get; set; }
        public Paged<MessagesTheme> List { get; set; }
        public MessagesTheme Theme { get; set;}
        public MessagesModel Item { get; set; }
    }
}