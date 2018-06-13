using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.entity
{
    public class NewsModel
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
        [RegularExpression(@"^[^-]([a-zA-Z0-9-]+)$", ErrorMessage = "Поле «alias» может содержать только буквы латинского алфавита и символ - (дефис). Доменное имя не может начинаться с дефиса.")]
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
        public bool Important { get; set; }
        public Guid[] CategoryId { get; set; }
        public NewsCategoryModel[] Category {get;set;}
        /// <summary>
        /// Ссылка на новость
        /// </summary>
        public string LinkNews { get; set; }
    }

    public class NewsCategoryModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public int Sort { get; set; }
        public Guid SiteId { get; set; }
    }   
}
