using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.Entity.modules.messages
{
    public class MessagesFilter: FilterModel
    {
        public bool? ViewMessages { get; set; }
    }
}
