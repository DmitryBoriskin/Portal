using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.Entity.common
{
    public class LayoutModel
    {
        /// <summary>
        /// Главное меню
        /// </summary>
        //public PageModel[] MainMenu { get; set; }  
        /// <summary>
        /// Подключенный лицевой счет
        /// </summary>
        public SubscrModel[] ConnectionSubscrList { get; set; }
        /// <summary>
        /// Выбранный лицевой счет
        /// </summary>
        public SubscrModel DefaultSubscr { get; set; }
    }
}
