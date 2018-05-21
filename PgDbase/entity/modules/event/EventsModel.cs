using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.entity
{
    public class EventsModel
    {
        /// <summary>
        /// Идентификатор в виде порядкого номера
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Идентификатор в бд
        /// </summary>
        public Guid Guid { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateEnd { get; set; }
        [RegularExpression(@"^([a-zA-Z0-9-]+)$", ErrorMessage = "Поле «alias» может содержать только буквы латинского алфавита и символ - (дефис). Доменное имя не может начинаться с дефиса.")]
        public string Alias { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Photo { get; set; }
        public string Keyw { get; set; }
        public string Desc { get; set; }
        public string SourceUrl { get; set; }
        public string SourceName { get; set; }
        public int ViewCount { get; set; }
        public bool Disabled { get; set; }
        /// <summary>
        /// Признак ежегодности
        /// </summary>
        public bool Annual { get; set; }

        /// <summary>
        /// прицепленные новости
        /// </summary>
        public NewsModel[] NewsInclude { get; set; }
    }
}
