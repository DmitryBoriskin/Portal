using PgDbase.entity;
using Portal.Areas.Admin.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MessagesModule.Areas.Admin.Models
{
    public class MessagesViewModel : CoreViewModel
    {
        public MessagesModel Item { get; set; }
        public Paged<MessagesTheme> List { get; set; }
        /// <summary>
        /// история сообщений
        /// </summary>
        public List<MessagesModel> MessageHistory { get; set; }
        /// <summary>
        /// список пользователей
        /// </summary>
        public IEnumerable<SelectListItem> UserList { get; set; }
    }
}