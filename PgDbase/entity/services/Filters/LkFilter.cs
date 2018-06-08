using System;
using System.Linq;

namespace PgDbase.entity
{
    /// <summary>
    /// Фильтр для личного кабинета
    /// </summary>
    public class LkFilter : FilterModel
    {
        /// <summary>
        /// Оплаченность
        /// </summary>
        public bool? Payed { get; set; }
    }
}
