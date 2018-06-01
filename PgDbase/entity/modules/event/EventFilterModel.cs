using PgDbase.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgDbase.entity
{
    /// <summary>
    /// Фильтр для карты сайта
    /// </summary>
    public class EventFilterModel : FilterModel
    {
        /// <summary>
        /// Родитель
        /// </summary>
        public bool? Annual { get; set; }
    }
}
