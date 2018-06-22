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
        //идентифкатор пользователя которое отправило сообщение
        public Guid User { get; set; }
        [Display(Name="Адресат")]
        public Guid? UserDestination { get; set; }
        public Guid? ParentId { get; set; }
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


    public class MessagesTheme: MessagesModel
    {
        public int AllCount { get; set; }
        public int NewMsgCount { get; set; }
        /// <summary>
        /// Участники сообщений
        /// </summary>
        public string MemberMsg { get; set; }
        public MessagesModel[] MessageList { get; set; }
    }
}
