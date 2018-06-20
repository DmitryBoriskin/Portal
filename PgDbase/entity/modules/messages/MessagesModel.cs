using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.entity
{
    public class MessagesModel
    {
        public Guid id { get; set; }        
        public Guid User { get; set; }
        public Guid Parent { get; set; }
        public string Theme { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
    }
}
