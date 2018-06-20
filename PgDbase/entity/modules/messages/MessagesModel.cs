using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.entity
{
    public class MessagesModel
    {
        public Guid Id { get; set; }        
        public Guid User { get; set; }
        public Guid ParentId { get; set; }
        /// <summary>
        /// тема сообщения
        /// </summary>
        public string Theme { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        /// <summary>
        /// признак прочитанности сообщения
        /// </summary>
        public bool View { get; set; }
        /// <summary>
        /// история сообщений
        /// </summary>
        public MessagesModel MessageHistory{ get; set; }
    }
}
