using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public string Text { get; set; }
        public DateTime Date { get; set; }
        /// <summary>
        /// признак прочитанности сообщения
        /// </summary>
        public bool View { get; set; }
        /// <summary>
        /// Признак того что сообщение написано из админки
        /// </summary>
        public bool Admin { get; set; }
        /// <summary>
        /// Автор сообщений
        /// </summary>
        public UserModel MsgUser { get; set; }
        


    }
}
