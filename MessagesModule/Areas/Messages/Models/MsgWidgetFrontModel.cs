using PgDbase.entity;
using Portal.Models;
using System.Collections.Generic;

namespace MessagesModule.Areas.Messages.Models
{
    public class MsgWidgetFrontModel
    {        
        //новые сообщения
        public List<MessagesTheme> MsgList { get; set; }
        //количество новых сообщений
        public string NewMsgCountText { get; set; } = "У Вас нет новых сообщений";
        public int NewMsgCount { get; set; } = 0;
    }
}