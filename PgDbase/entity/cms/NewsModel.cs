using System;
using System.Collections.Generic;
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
    }


}
